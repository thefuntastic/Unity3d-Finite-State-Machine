using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	public class TestMonoBehaviourUpdates
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
		public IEnumerator TestUpdate()
		{
			fsm.ChangeState(States.One);

			yield return null;

			Assert.AreEqual(1, behaviour.oneEnter);
			Assert.AreEqual(1, behaviour.oneUpdate);
			Assert.AreEqual(1, behaviour.oneLateUpdate);

			yield return new WaitForSeconds(Time.fixedDeltaTime);

			Assert.GreaterOrEqual(behaviour.oneFixedUpdate, 1);
		}

		private class StateClass : MonoBehaviour
		{
			public int oneEnter;
			public int oneUpdate;
			public int oneFixedUpdate;
			public int oneLateUpdate;
			public int oneExit;
			public int oneFinally;

			void One_Enter()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Enter", Time.frameCount);
				oneEnter++;
			}

			void One_FixedUpdate()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One FixedUpdate", Time.frameCount);
				oneFixedUpdate++;
			}

			void One_Update()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Update", Time.frameCount);
				oneUpdate++;
			}

			void One_LateUpdate()
			{
				Debug.LogFormat("State:{0} Frame:{1}", "One Late Update", Time.frameCount);
				oneLateUpdate++;
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
		}
	}
}