using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

[TestFixture]
[Category("State Machine Tests")]
internal class TestBasicTransitions 
{
	public enum States
	{
		One,
		Two,
		Three,
	}

	private GameObject go;
	private ClassWithBasicStates behaviour;
	private StateEngine engine;
	private StateMachine<States> fsm;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<ClassWithBasicStates>();
		engine = go.AddComponent<StateEngine>();
		fsm = engine.Initialize<States>(behaviour);
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void NoTransitions()
	{
		//Goes to start state by default;
		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.oneStats.exitCount);

		Assert.AreEqual(0, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.twoStats.exitCount);

		Assert.AreEqual(0, behaviour.threeStats.enterCount);
		Assert.AreEqual(0, behaviour.threeStats.updateCount);
		Assert.AreEqual(0, behaviour.threeStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.threeStats.exitCount);
	}


	[Test]
	public void InitialTransition()
	{
		fsm.ChangeState(States.Two);

		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.oneStats.exitCount);

		Assert.AreEqual(1, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.twoStats.exitCount);

		Assert.AreEqual(0, behaviour.threeStats.enterCount);
		Assert.AreEqual(0, behaviour.threeStats.updateCount);
		Assert.AreEqual(0, behaviour.threeStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.threeStats.exitCount);
	}


	[Test]
	public void IgnoreMultipleTransitions()
	{
		fsm.ChangeState(States.Two);
		fsm.ChangeState(States.Two);

		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.oneStats.exitCount);

		Assert.AreEqual(1, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.twoStats.exitCount);

		Assert.AreEqual(0, behaviour.threeStats.enterCount);
		Assert.AreEqual(0, behaviour.threeStats.updateCount);
		Assert.AreEqual(0, behaviour.threeStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.threeStats.exitCount);
	}


	[Test]
	public void MultipleTransitions()
	{
		fsm.ChangeState(States.One);

		fsm.ChangeState(States.Two);

		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.oneStats.exitCount);

		Assert.AreEqual(1, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.twoStats.exitCount);

		Assert.AreEqual(0, behaviour.threeStats.enterCount);
		Assert.AreEqual(0, behaviour.threeStats.updateCount);
		Assert.AreEqual(0, behaviour.threeStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.threeStats.exitCount);

		fsm.ChangeState(States.Three);

		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.oneStats.exitCount);

		Assert.AreEqual(1, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.twoStats.exitCount);

		Assert.AreEqual(1, behaviour.threeStats.enterCount);
		Assert.AreEqual(0, behaviour.threeStats.updateCount);
		Assert.AreEqual(0, behaviour.threeStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.threeStats.exitCount);
		
	}
	

}	


