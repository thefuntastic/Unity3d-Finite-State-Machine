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
}	


