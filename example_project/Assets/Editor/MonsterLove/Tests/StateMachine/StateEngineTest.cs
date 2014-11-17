using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityTest;

[TestFixture]
[Category("State Machine Tests")]
internal class StateEngineTest : UnityUnitTest
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

	private StateMachineBehaviour behaviour;
	private StateMachineEngine engine;

	[SetUp]
	public void Init()
	{
		var go = CreateGameObject("stateTest");
		behaviour = go.AddComponent<StateMachineBehaviour>();
		engine = go.GetComponent<StateMachineEngine>();
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
		engine.Initialize<StateMachineBehaviour, TestState>(behaviour);

		engine.ChangeState(TestTrollStates.Bogey);
	}

	[Test]
	[ExpectedException]
	public void TestNotInitialized()
	{
		engine.ChangeState(TestStates.StateInit);
	}

}	


