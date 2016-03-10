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

	public enum TestNoDefines
	{
	}

	private GameObject go;
	private MonoBehaviour behaviour;
	private StateMachineRunner engine;

	[SetUp]
	public void Init()
	{
		go = new GameObject("stateTest");
		behaviour = go.AddComponent<MonoBehaviour>();
		engine = go.AddComponent<StateMachineRunner>();
		
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	//[ExpectedException]
	public void TestInitializedTwice()
	{ 
		//Should this throw an error? I'm not sure?
		var fsm = engine.Initialize<TestStates>(behaviour);
		fsm = engine.Initialize<TestStates>(behaviour);
	}

	[Test]
	[ExpectedException]
	public void TestStatesDefined()
	{
		var fsm = engine.Initialize<TestNoDefines>(behaviour);
	}
}	


