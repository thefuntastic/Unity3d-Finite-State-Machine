using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;

public class TestEmptyDriver
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
	public void TestEmptyDriverUpdate()
	{
		fsm.ChangeState(States.One);

		Assert.AreEqual(1, behaviour.oneEnter);
		Assert.AreEqual(0, behaviour.oneExit);

		Assert.AreEqual(0, behaviour.twoEnter);
		Assert.AreEqual(0, behaviour.twoExit);
		
		fsm.ChangeState(States.Two);

		Assert.AreEqual(1, behaviour.oneEnter);
		Assert.AreEqual(1, behaviour.oneExit);

		Assert.AreEqual(1, behaviour.twoEnter);
		Assert.AreEqual(0, behaviour.twoExit);
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