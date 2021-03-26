using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Collections;
using Object = UnityEngine.Object;

[TestFixture]
[Category("State Machine Tests")]
internal class TestDerivedFromSuperClass 
{
	public enum States
	{
		One,
		Two,
		Three,
	}

	private GameObject go;
	private ClassWithBasicStates behaviour;
	private StateMachineRunner engine;
	private StateMachine<States> fsm;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<ClassDerivedFromSuperClass>();
		engine = go.AddComponent<StateMachineRunner>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void InitialTransition()
	{
		fsm = engine.Initialize<States>(behaviour, States.One);
		fsm.ChangeState(States.Two);

		//Test for when we want to include superclass methods
		Assert.AreEqual(1, behaviour.oneStats.enterCount);
		Assert.AreEqual(0, behaviour.oneStats.updateCount);
		Assert.AreEqual(0, behaviour.oneStats.lateUpdateCount);
		Assert.AreEqual(1, behaviour.oneStats.exitCount);
		Assert.AreEqual(1, behaviour.oneStats.finallyCount);

		Assert.AreEqual(1, behaviour.twoStats.enterCount);
		Assert.AreEqual(0, behaviour.twoStats.updateCount);
		Assert.AreEqual(0, behaviour.twoStats.lateUpdateCount);
		Assert.AreEqual(0, behaviour.twoStats.exitCount);
		Assert.AreEqual(0, behaviour.twoStats.finallyCount);
	}
}	


