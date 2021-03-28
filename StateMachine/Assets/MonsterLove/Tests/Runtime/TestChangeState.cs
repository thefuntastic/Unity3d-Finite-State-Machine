using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class TestChangeState
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
		private StateMachine<States> fsm;

		[SetUp]
		public void Init()
		{
			go = new GameObject();
			behaviour = go.AddComponent<StateClass>();

			fsm = StateMachine<States>.Initialize(behaviour);
		}

		[TearDown]
		public void Kill()
		{
			Object.Destroy(go);
		}

		[UnityTest]
		public IEnumerator TestChange()
		{
			fsm.ChangeState(States.One);
			
			Assert.Catch(()=>
			{
				var last = fsm.LastState;
			});
			Assert.AreEqual(States.One, fsm.State);
			Assert.AreEqual(States.One, fsm.NextState);

			yield return null;

			fsm.ChangeState(States.Two);

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(1, behaviour.oneUpdate);
			Assert.AreEqual(1, behaviour.oneExit);
			Assert.AreEqual(1, behaviour.oneFinally);
			Assert.AreEqual(1, behaviour.twoEnter);
			Assert.AreEqual(0, behaviour.twoUpdate); //Only changed this frame, hasn't had a chance to update yet

			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Two, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);
			
			yield return null;

			Assert.AreEqual(1, behaviour.twoUpdate); //First update runs a frame later
		}

		[UnityTest]
		public IEnumerator TestChangeSameFrame()
		{
			fsm.ChangeState(States.One);
			fsm.ChangeState(States.Two);

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate); //Update never has chance to fire during same frame change 
			Assert.AreEqual(1, behaviour.oneExit);
			Assert.AreEqual(1, behaviour.oneFinally);
			Assert.AreEqual(1, behaviour.twoEnter);
			Assert.AreEqual(0, behaviour.twoUpdate);
			
			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Two, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);


			yield return null;

			Assert.AreEqual(0, behaviour.oneUpdate);
			Assert.AreEqual(1, behaviour.twoUpdate); //First frame runs in state two 
		}

		private class StateClass : MonoBehaviour
		{
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

			void One_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Enter", Time.frameCount);
				oneEnter++;
			}

			void One_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Update", Time.frameCount);
				oneUpdate++;
			}

			void One_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Exit", Time.frameCount);
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