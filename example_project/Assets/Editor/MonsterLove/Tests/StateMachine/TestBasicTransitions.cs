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
	private GameObject go;
	private ClassWithBasicStates behaviour;
	private StateEngine engine;
	private StateMachine<ClassWithBasicStates.States> fsm;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<ClassWithBasicStates>();
		engine = go.AddComponent<StateEngine>();
		fsm = engine.Initialize<ClassWithBasicStates.States>(behaviour);
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void NoTransitions()
	{
		Assert.AreEqual(0, behaviour.oneStats.enterCount);
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
		fsm.ChangeState(ClassWithBasicStates.States.One);

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
	public void MultipleTransitions()
	{
		fsm.ChangeState(ClassWithBasicStates.States.One);

		fsm.ChangeState(ClassWithBasicStates.States.Two);

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

		fsm.ChangeState(ClassWithBasicStates.States.Three);

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


