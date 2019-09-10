using System;
using System.Collections;
using System.Collections.Generic;

namespace MonsterLove.StateMachine
{
	internal class StateMapping<TState, TDriver> where TState : struct, IConvertible, IComparable where TDriver : class, new()
	{
		public TState state;
		public TDriver driver;
		
		public bool hasEnterRoutine;
		public Action EnterCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> EnterRoutine = StateMachineRunner.DoNothingCoroutine;

		public bool hasExitRoutine;
		public Action ExitCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> ExitRoutine = StateMachineRunner.DoNothingCoroutine;

		public Action Finally = StateMachineRunner.DoNothing;

		private StateMachine<TState, TDriver> fsm;

		public StateMapping(StateMachine<TState, TDriver> fsm, TState state)
		{
			this.fsm = fsm;
			this.state = state;
			driver = new TDriver();
		}

		public bool TestInvokable()
		{
			if (fsm.IsInTransition)
			{
				return false;
			}

			if (!fsm.IsCurrentState(this))
			{
				return false;
			}

			return true;
		}
	}
}