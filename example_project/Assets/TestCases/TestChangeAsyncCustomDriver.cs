using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class TestAsyncCustomDriver
	{
		public enum States
		{
			One,
			Two,
			Three,
			Four,
		}

		private GameObject go;
		private StateClass behaviour;
		private StateMachine<States, StateDriverUnity> fsm;

		[SetUp]
		public void Init()
		{
			go = new GameObject();
			behaviour = go.AddComponent<StateClass>();

			fsm = new StateMachine<States, StateDriverUnity>(behaviour);
			behaviour.fsm = fsm;
		}

		[TearDown]
		public void Kill()
		{
			Object.Destroy(go);
		}

		[UnityTest]
		public IEnumerator TestChange()
		{
			// /1
			fsm.ChangeState(States.One);

			yield return null;

			Assert.AreEqual(0, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate); //Enter still running, no update called

			yield return null;

			Assert.AreEqual(1, behaviour.oneEnter);  //Enter now complete
			Assert.AreEqual(0, behaviour.oneUpdate); //Update executes before co-routine, therefore update not run yet

			yield return null;
			
			Assert.AreEqual(1, behaviour.oneEnter);  
			Assert.AreEqual(1, behaviour.oneUpdate); //Update has first chance to run
		}

		private class StateClass : MonoBehaviour
		{
			public StateMachine<States, StateDriverUnity> fsm;

			public int oneEnter;
			public int oneUpdate;
			public int oneExit;
			public int oneFinally;

			public int twoEnter;
			public int twoUpdate;
			public int twoExit;
			public int twoFinally;

			public int threeEnter;
			public int threeUpdate;
			public int threeExit;
			public int threeFinally;

			public int fourEnter;
			public int fourUpdate;
			public int fourExit;
			public int fourFinally;

			void Update()
			{
				fsm.Driver.Update.Invoke();
			}

			IEnumerator One_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Enter Start", Time.frameCount);

				yield return null;
				yield return null;

				Debug.LogFormat("State:{0} Frame:{1}", "One Enter End", Time.frameCount);

				oneEnter++;
			}

			void One_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Update", Time.frameCount);
				oneUpdate++;
			}

			IEnumerator One_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Exit Start", Time.frameCount);

				yield return null;
				yield return null;

				Debug.LogFormat("State:{0} Frame:{1}", "One Exit End", Time.frameCount);

				oneExit++;
			}

			void One_Finally()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Finally", Time.frameCount);
				oneFinally++;
			}

			void Two_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Two Enter", Time.frameCount);
				twoEnter++;
			}

			void Two_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Two Update", Time.frameCount);
				twoUpdate++;
			}

			void Two_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
				twoExit++;
			}

			void Two_Finally()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Two Finally", Time.frameCount);
				twoFinally++;
			}

			void Three_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Three Enter", Time.frameCount);
				threeEnter++;
			}

			void Three_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Three Update", Time.frameCount);
				threeUpdate++;
			}

			void Three_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Three Exit", Time.frameCount);
				threeExit++;
			}

			void Three_Finally()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Three Finally", Time.frameCount);
				threeFinally++;
			}

			void Four_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Four Enter", Time.frameCount);
				fourEnter++;
			}

			void Four_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Four Update", Time.frameCount);
				fourUpdate++;
			}

			void Four_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Four Exit", Time.frameCount);
				fourExit++;
			}

			void Four_Finally()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Four Finally", Time.frameCount);
				fourFinally++;
			}
		}
	}
}