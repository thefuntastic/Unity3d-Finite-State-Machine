using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class TestChangeAsyncOverwrite
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
		private float duration = 0.5f;

		[SetUp]
		public void Init()
		{
			go = new GameObject();
			behaviour = go.AddComponent<StateClass>();
			behaviour.duration = duration;

			fsm = StateMachine<States>.Initialize(behaviour);
		}

		[TearDown]
		public void Kill()
		{
			Object.Destroy(go);
		}

		[UnityTest]
		public IEnumerator TestAsyncEnterExit()
		{
			// 1
			fsm.ChangeState(States.One, StateTransition.Overwrite);

			// 1*__/2
			fsm.ChangeState(States.Two, StateTransition.Overwrite);
			
			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Two, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);

			yield return new WaitForSeconds(duration + 0.2f);

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate);
			Assert.AreEqual(0, behaviour.oneExit);
			Assert.AreEqual(1, behaviour.oneFinally);

			Assert.AreEqual(1, behaviour.twoEnter);
			
			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Two, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);
		}

		[UnityTest]
		public IEnumerator TestChangeDuringAsyncEnter()
		{
			// 1
			fsm.ChangeState(States.One, StateTransition.Overwrite);

			// 1\__/2
			fsm.ChangeState(States.Two, StateTransition.Safe);

			Assert.AreEqual(false, fsm.LastStateExists);
			Assert.AreEqual(States.One, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);
			
			yield return new WaitForSeconds(duration + duration * 0.5f);
			
			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate);
			Assert.AreEqual(1, behaviour.oneExit);
			Assert.AreEqual(1, behaviour.oneFinally);

			Assert.AreEqual(0, behaviour.twoEnter);

			Assert.AreEqual(0, behaviour.threeEnter);
			
			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Two, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);

			// 1\__*2__3
			fsm.ChangeState(States.Three, StateTransition.Overwrite);

			Assert.AreEqual(0, behaviour.twoEnter); //Intro never completes
			Assert.AreEqual(1, behaviour.twoExit); //Single frame Exit still fires. Arguably more consistent that exit doesn't fire, instead relying on finally(), however this would be a breaking change 
			Assert.AreEqual(0, behaviour.twoUpdate);
			Assert.AreEqual(1, behaviour.twoFinally);

			Assert.AreEqual(1, behaviour.threeEnter);
			
			Assert.AreEqual(States.Two, fsm.LastState);
			Assert.AreEqual(States.Three, fsm.State);
			Assert.AreEqual(States.Three, fsm.NextState);
		}

		[UnityTest]
		public IEnumerator TestChangeDuringAsyncExit()
		{
			// 1
			fsm.ChangeState(States.One, StateTransition.Overwrite);

			// 1\__/2
			fsm.ChangeState(States.Two, StateTransition.Safe);

			Assert.AreEqual(false, fsm.LastStateExists);
			Assert.AreEqual(States.One, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);
			
			yield return new WaitForSeconds(duration * 0.5f);

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate);
			Assert.AreEqual(0, behaviour.oneExit);
			Assert.AreEqual(0, behaviour.oneFinally);

			Assert.AreEqual(0, behaviour.twoEnter);

			Assert.AreEqual(false, fsm.LastStateExists);
			Assert.AreEqual(States.One, fsm.State);
			Assert.AreEqual(States.Two, fsm.NextState);
			
			// 1*__/4
			fsm.ChangeState(States.Four, StateTransition.Overwrite);

			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Four, fsm.State);
			Assert.AreEqual(States.Four, fsm.NextState);
			
			yield return new WaitForSeconds(duration + 0.2f);

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(0, behaviour.oneUpdate);
			Assert.AreEqual(0, behaviour.oneExit); //Cancelled - never completed
			Assert.AreEqual(1, behaviour.oneFinally);

			Assert.AreEqual(0, behaviour.twoEnter); //Never entered

			Assert.AreEqual(1, behaviour.fourEnter);
			
			Assert.AreEqual(States.One, fsm.LastState);
			Assert.AreEqual(States.Four, fsm.State);
			Assert.AreEqual(States.Four, fsm.NextState);
		}

		private class StateClass : MonoBehaviour
		{
			public float duration;

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

			IEnumerator One_Exit()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Exit Start", Time.frameCount);
				yield return new WaitForSeconds(duration);
				Debug.LogFormat("State:{0} Frame:{1}", "One Exit End", Time.frameCount);
				oneExit++;
			}

			void One_Finally()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Finally", Time.frameCount);
				oneFinally++;
			}

			IEnumerator Two_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Two Enter Start", Time.frameCount);
				yield return new WaitForSeconds(duration);
				Debug.LogFormat("State:{0} Frame:{1}", "Two Enter End", Time.frameCount);
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

			IEnumerator Four_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "Four Enter Start", Time.frameCount);
				yield return new WaitForSeconds(duration);
				Debug.LogFormat("State:{0} Frame:{1}", "Four Enter End", Time.frameCount);
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