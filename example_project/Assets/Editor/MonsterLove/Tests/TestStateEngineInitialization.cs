using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
[Category("State Machine Tests")]
public class TestStateEngineInitialization 
{

	public enum TestStates
	{
		StateInit,
		StatePlay,
		StateEnd,
	}

	public enum TestNoDefines
	{
	}

	private GameObject go;
	private ClassWithBasicStates behaviour;
	private StateMachineRunner engine;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<ClassWithBasicStates>();
		engine = go.AddComponent<StateMachineRunner>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestInitializedTwice()
	{ 
		//Should this throw an error? I'm not sure?
		var fsm = engine.Initialize<TestStates>(behaviour);
		fsm = engine.Initialize<TestStates>(behaviour);
	}

	[Test]
	public void TestStatesDefined()
	{
		Assert.Throws<ArgumentException>(
					  () => { engine.Initialize<TestNoDefines>(behaviour); }
					 );
	}

	[Test]
	public void TestStatePropBeforeChange()
	{
		var fsm = new StateMachine<TestStates, StateDriverUnity>(behaviour);
		
		Assert.Throws<NullReferenceException>(() =>
		{
			TestStates state = fsm.State;
		});
		
		fsm.ChangeState(TestStates.StateInit);
		
		Assert.AreEqual(TestStates.StateInit, fsm.State);
	}
	
	[Test]
	public void TestLastStatePropBeforeChange()
	{
		var fsm = new StateMachine<TestStates, StateDriverUnity>(behaviour);
		
		Assert.Throws<NullReferenceException>(() =>
		{
			TestStates state = fsm.LastState;
		});
		Assert.IsFalse(fsm.LastStateExists);
		
		
		fsm.ChangeState(TestStates.StateInit);
		
		//Conflicted about this. Prefer to return default values, or the current state
		//but that would undermine correctness
		Assert.Throws<NullReferenceException>(() =>
		{
			TestStates state = fsm.LastState;
		});
		Assert.IsFalse(fsm.LastStateExists);
		
		fsm.ChangeState(TestStates.StatePlay);
		
		Assert.AreEqual(TestStates.StateInit, fsm.LastState);
		Assert.IsTrue(fsm.LastStateExists);
	}
}	


