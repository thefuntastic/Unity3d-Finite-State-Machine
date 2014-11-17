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

				return _stateMachine;
			}
		}
	}
}
