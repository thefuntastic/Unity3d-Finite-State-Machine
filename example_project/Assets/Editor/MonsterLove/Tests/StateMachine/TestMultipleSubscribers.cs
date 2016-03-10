using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

[TestFixture]
[Category("State Machine Tests")]
internal class TestMultipleSubscribers 
{
	public enum States
	{
		One,
		Two,
		Three,
	}

	private GameObject go;
	private ClassWithBasicStates behaviour1;
	private ClassWithBasicStates behaviour2;
	private ClassWithBasicStates behaviour3;
	private StateMachineRunner engine;
	private StateMachine<States> fsm;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour1 = go.AddComponent<ClassWithBasicStates>();
		behaviour2 = go.AddComponent<ClassWithBasicStates>();
		behaviour3 = go.AddComponent<ClassWithBasicStates>();
		engine = go.AddComponent<StateMachineRunner>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestNoCrossTalk()
	{
		var fsm1 = engine.Initialize<States>(behaviour1, States.One);
		var fsm2 = engine.Initialize<States>(behaviour2, States.Two);
		var fsm3 = engine.Initialize<States>(behaviour3, States.One);

		fsm2.ChangeState(States.Three);
		fsm2.ChangeState(States.Two);

		fsm3.ChangeState(States.Three);

		
		Assert.AreEqual(1, behaviour1.oneStats.enterCount);
		Assert.AreEqual(0, behaviour1.oneStats.exitCount);
		Assert.AreEqual(0, behaviour1.twoStats.enterCount);
		Assert.AreEqual(0, behaviour1.twoStats.exitCount);
		Assert.AreEqual(0, behaviour1.threeStats.enterCount);
		Assert.AreEqual(0, behaviour1.threeStats.exitCount);

		Assert.AreEqual(0, behaviour2.oneStats.enterCount);
		Assert.AreEqual(0, behaviour2.oneStats.exitCount);
		Assert.AreEqual(2, behaviour2.twoStats.enterCount);
		Assert.AreEqual(1, behaviour2.twoStats.exitCount);
		Assert.AreEqual(1, behaviour2.threeStats.enterCount);
		Assert.AreEqual(1, behaviour2.threeStats.exitCount);

		Assert.AreEqual(1, behaviour3.oneStats.enterCount);
		Assert.AreEqual(1, behaviour3.oneStats.exitCount);
		Assert.AreEqual(0, behaviour3.twoStats.enterCount);
		Assert.AreEqual(0, behaviour3.twoStats.exitCount);
		Assert.AreEqual(1, behaviour3.threeStats.enterCount);
		Assert.AreEqual(0, behaviour3.threeStats.exitCount);
	}


}	


