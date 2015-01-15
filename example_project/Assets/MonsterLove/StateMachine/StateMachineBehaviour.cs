using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterLove.StateMachine
{
	[RequireComponent(typeof(StateMachineEngine))]
	public class StateMachineBehaviour : MonoBehaviour
	{
		private StateMachineEngine _stateMachine;

		
		public StateMachineEngine stateMachine
		{
			get
			{
				if(_stateMachine == null)
				{
					//Guaranteed to be availble thanks to RequireComponent
					_stateMachine = GetComponent<StateMachineEngine>();
				}

				//This happens when we forget to inherit from StateMachineBehaviour and change it after the script has been added to a game object.
				if(_stateMachine == null)
				{
					throw new Exception("Please make sure StateMachineEngine is also present on any StateMachineBehaviour objects");
				}

				return _stateMachine;
			}
		}

		public Enum GetState()
		{
			return stateMachine.GetState();
		}

		protected void Initialize<T>()
		{
			stateMachine.Initialize<T>(this);
		}

		protected void ChangeState(Enum newState)
		{
			stateMachine.ChangeState(newState);
		}
	}
}
