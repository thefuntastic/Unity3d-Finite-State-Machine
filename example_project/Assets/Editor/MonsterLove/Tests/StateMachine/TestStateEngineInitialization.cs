using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityTest;
using Object = UnityEngine.Object;

[TestFixture]
[Category("State Machine Tests")]
internal class TestStateEngineInitialization 
{

	public enum TestStates
	{
		StateInit,
		StatePlay,
		StateEnd,
	}

	public enum TestTrollStates
	{
		Bogey, 
		Gremlin, 
	}

	private GameObject go;
	private StateBehaviour behaviour;
	private StateEngine engine;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<StateBehaviour>();
		engine = go.GetComponent<StateEngine>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestBehaviourHasEngine()
	{
		Assert.IsNotNull(behaviour.stateMachine);
	}

	[Test]
	[ExpectedException]
	public void TestEnumNotUsedForInit()
	{
		engine.Initialize<TestState>(behaviour);

		engine.ChangeState(TestTrollStates.Bogey);
	}

	[Test]
	//[ExpectedException]
	public void TestInitializedTwice()
	{
		//Should this be an exception or is this a legimate use case? I'm not sure

		engine.Initialize<TestState>(behaviour);

		engine.Initialize<TestTrollStates>(behaviour);
	}

	[Test]
	[ExpectedException]
	public void TestNotInitialized()
	{
		engine.ChangeState(TestStates.StateInit);
	}

}	


