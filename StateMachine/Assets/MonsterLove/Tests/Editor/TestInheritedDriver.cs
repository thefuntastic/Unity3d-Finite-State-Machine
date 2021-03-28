using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;

public class TestInheritedDriver
{
	public enum States
	{
		One,
		Two,
		Three,
		Four,
	}

	public class Driver : StateDriverUnity
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
	public void TestDriverNotNull()
	{
		Assert.NotNull(fsm.Driver);
	}

	[Test]
	public void TestCustomEvents()
	{
		fsm.ChangeState(States.One);

		fsm.Driver.Foo.Invoke();
		fsm.Driver.Update.Invoke();

		Assert.AreEqual(1, behaviour.oneFoo);
		Assert.AreEqual(1, behaviour.oneUpdate);

		Assert.AreEqual(0, behaviour.twoFoo);
		Assert.AreEqual(0, behaviour.twoUpdate);

		fsm.ChangeState(States.Two);

		fsm.Driver.Foo.Invoke();
		fsm.Driver.Update.Invoke();

		Assert.AreEqual(1, behaviour.oneFoo);
		Assert.AreEqual(1, behaviour.oneUpdate);

		Assert.AreEqual(1, behaviour.twoFoo);
		Assert.AreEqual(1, behaviour.twoUpdate);
	}

	private class StateClass : MonoBehaviour
	{
		public int oneEnter;
		public int oneFoo;
		public int oneUpdate;
		public int oneExit;

		public int twoEnter;
		public int twoFoo;
		public int twoUpdate;
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

		void One_Update()
		{
			oneUpdate++;
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
		
		void Two_Update()
		{
			twoUpdate++;
		}
		
		void Two_Exit()
		{
			//Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
			twoExit++;
		}
	}
}