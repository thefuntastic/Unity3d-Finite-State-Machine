using System;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class TestStateClassTypesMismatchDriver
{
	public enum States
	{
		One,
		Two,
		Three,
		Four,
	}

	public class Driver
	{
		public StateEvent Foo;
		public StateEvent<int> Bar;
		public StateEvent<int, int> Baz;
	}

	private GameObject go;
	private StateClass behaviour;
	private StateMachine<States, Driver> fsm;

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
	public void TestMismatchedEvents()
	{
		Assert.Throws<ArgumentException>(() =>
									 new StateMachine<States, Driver>(behaviour)
								);
	}

	private class StateClass : MonoBehaviour
	{
		public int oneEnter;
		public int oneFoo;
		public int oneBar;
		public int oneBaz;
		public int oneExit;

		public int oneBarValue;
		public int oneBazValueA;
		public int oneBazValueB;

		public int twoEnter;
		public int twoExit;

		void One_Enter()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Enter", Time.frameCount);
			oneEnter++;
		}

		void One_Baz()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Foo", Time.frameCount);
			oneFoo++;
		}

		void One_Foo(int value)
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Bar", Time.frameCount);
			oneBar++;
			oneBarValue = value;
		}

		void One_Bar(int valueA, int valueB)
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Baz", Time.frameCount);
			oneBaz++;
			oneBazValueA = valueA;
			oneBazValueB = valueB;
		}

		void One_Exit()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Exit", Time.frameCount);
			oneExit++;
		}

		void Two_Enter()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Enter", Time.frameCount);
			twoEnter++;
		}

		void Two_Exit()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
			twoExit++;
		}
	}
}