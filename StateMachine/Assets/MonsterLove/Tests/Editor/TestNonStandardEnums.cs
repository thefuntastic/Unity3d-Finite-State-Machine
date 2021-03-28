using System;
using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class TestNonStandardEnums
{
	private enum StatesUlong : ulong
	{
		Foo = ulong.MaxValue,
	}
	
	private GameObject go;
	private StateClass behaviour;
	private StateMachine<StatesUlong, TestCachedDriver.Driver> fsm;
	
	[SetUp]
	public void Init()
	{
		go = new GameObject();
		behaviour = go.AddComponent<StateClass>();
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestNonIntEnumErrors()
	{
		Assert.Catch(typeof(ArgumentException), () =>
		{
			fsm = new StateMachine<StatesUlong, TestCachedDriver.Driver>(behaviour);
		});
	}

	private class StateClass : MonoBehaviour
	{
		
	}
}