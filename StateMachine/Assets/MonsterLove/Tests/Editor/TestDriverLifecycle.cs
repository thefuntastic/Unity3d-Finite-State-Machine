using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class TestDriverLifecycle
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
	}

	private GameObject go;
	private StateClass behaviour;
	private StateMachine<States, Driver> fsm;

	[SetUp]
	public void Init()
	{
		go = new GameObject();
		behaviour = go.AddComponent<StateClass>();

		fsm = new StateMachine<States, Driver>(behaviour);
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestDriverEventDoesntFireBeforeStateSet()
	{
		fsm.Driver.Foo.Invoke();

		Assert.AreEqual(0, behaviour.oneFoo);

		fsm.ChangeState(States.One);

		fsm.Driver.Foo.Invoke();

		Assert.AreEqual(1, behaviour.oneFoo);
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
		public int twoFoo;
		public int twoBar;
		public int twoBaz;
		public int twoExit;

		public int twoBarValue;
		public int twoBazValueA;
		public int twoBazValueB;

		void One_Enter()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Enter", Time.frameCount);
			oneEnter++;
		}

		void One_Foo()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Foo", Time.frameCount);
			oneFoo++;
		}

		void One_Bar(int value)
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "One Bar", Time.frameCount);
			oneBar++;
			oneBarValue = value;
		}

		void One_Baz(int valueA, int valueB)
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

		void Two_Foo()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Foo", Time.frameCount);
			twoFoo++;
		}

		void Two_Bar(int value)
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Bar", Time.frameCount);
			twoBar++;
			twoBarValue = value;
		}

		void Two_Baz(int valueA, int valueB)
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Baz", Time.frameCount);
			twoBaz++;
			twoBazValueA = valueA;
			twoBazValueB = valueB;
		}

		void Two_Exit()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
			twoExit++;
		}
	}
}