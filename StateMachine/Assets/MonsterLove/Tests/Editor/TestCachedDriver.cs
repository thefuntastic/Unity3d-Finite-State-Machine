using System.Runtime.CompilerServices;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;

/// This is an important test, enables subscription directly to driver objects
///	Eg. button.OnClick.AddListener(fsm.Driver.Foo.Invoke)
public class TestCachedDriver
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
	private Driver driver;

	[SetUp]
	public void Init()
	{
		go = new GameObject();
		behaviour = go.AddComponent<StateClass>();

		fsm = new StateMachine<States, Driver>(behaviour);
		driver = fsm.Driver;
	}

	[TearDown]
	public void Kill()
	{
		Object.DestroyImmediate(go);
	}

	[Test]
	public void TestCachedDriverEvents()
	{
		fsm.ChangeState(States.One);

		driver.Foo.Invoke();

		Assert.AreEqual(1, behaviour.oneFoo);
		Assert.AreEqual(0, behaviour.twoFoo);

		fsm.ChangeState(States.Two);

		driver.Foo.Invoke();

		Assert.AreEqual(1, behaviour.oneFoo);
		Assert.AreEqual(1, behaviour.twoFoo);
	}

	private class StateClass : MonoBehaviour
	{
		public int oneEnter;
		public int oneFoo;
		public int oneExit;

		public int twoEnter;
		public int twoFoo;
		public int twoExit;

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

		void Two_Exit()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
			twoExit++;
		}
	}
}