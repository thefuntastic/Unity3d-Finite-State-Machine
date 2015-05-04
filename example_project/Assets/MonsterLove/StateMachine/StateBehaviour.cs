using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterLove.StateMachine
{
	[RequireComponent(typeof(StateEngine))]
	public class StateBehaviour : MonoBehaviour
	{
		private StateEngine _stateMachine;

		
		public StateEngine stateMachine
		{
			get
			{
				if(_stateMachine == null)
				{
					//Guaranteed to be availble thanks to RequireComponent
					_stateMachine = GetComponent<StateEngine>();
				}

				//This happens when we forget to inherit from StateBehaviour and change it after the script has been added to a game object.
				if(_stateMachine == null)
				{
					throw new Exception("Please make sure StateEngine is also present on any StateBehaviour objects");
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

		protected void ChangeState(Enum newState, StateMachineTransition transition)
		{
			stateMachine.ChangeState(newState, transition);
		}
	}
}
