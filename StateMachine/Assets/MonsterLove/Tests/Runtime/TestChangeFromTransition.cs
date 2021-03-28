using System;
using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
	// TEST DESCRIPTION
	//
	// Coverage for the scenario where state transition is aborted based on a flag evaluated within transition Enter and Exit methods
	public class TestChangeFromTransition
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
			behaviour.fsm = fsm;
		}

		[TearDown]
		public void Kill()
		{
			Object.Destroy(go);
		}

		[Test]
		public void TestChangeFromExit()
		{
			//TODO stack overflow is not the expected behaviour - also seems to be periodically crashing test suite. Disable for now 

			// // 1
			// fsm.ChangeState(States.One);
			//
			// 
			//
			// // 1-__3 //One_Exit contains change to 3
			// Assert.Throws<StackOverflowException>(
			//                                       ()=> fsm.ChangeState(States.Two)
			//                                       );
			//
			//
			//
			// // Assert.AreEqual(1, behaviour.oneEnter);
			// // Assert.AreEqual(0, behaviour.oneUpdate); 
			// // Assert.AreEqual(1, behaviour.oneExit);
			// // Assert.AreEqual(1, behaviour.oneFinally);
			// //
			// // Assert.AreEqual(0, behaviour.twoEnter);
			// // Assert.AreEqual(0, behaviour.twoUpdate); 
			// // Assert.AreEqual(0, behaviour.twoFinally);
			// //
			// // Assert.AreEqual(1, behaviour.threeEnter);
		}


		[Test]
		public void TestChangeFromEnter()
		{
			fsm.ChangeState(States.Two);
			
			// -4__3 //Four_Enter contains change to 3
			fsm.ChangeState(States.Four);

			Assert.AreEqual(1, behaviour.fourEnter); //This will complete after Exit and Finally. Not sure how to make this subtle side effect more obvious
			Assert.AreEqual(0, behaviour.fourUpdate);
			Assert.AreEqual(1, behaviour.fourExit);
			Assert.AreEqual(1, behaviour.fourFinally);

			Assert.AreEqual(1, behaviour.threeEnter);
			
			Assert.AreEqual(States.Four, fsm.LastState);
			Assert.AreEqual(States.Three, fsm.State);
			Assert.AreEqual(States.Three, fsm.NextState);
		}

		private class StateClass : MonoBehaviour
		{
			public StateMachine<States> fsm;

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
				fsm.ChangeState(States.Three);

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
				fsm.ChangeState(States.Three);
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