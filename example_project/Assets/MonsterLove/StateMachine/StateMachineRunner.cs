/*
 * Copyright (c) 2019 Made With Monster Love (Pty) Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterLove.StateMachine
{
	public class StateMachineRunner : MonoBehaviour
	{
		private List<IStateMachine<StateDriverRunner>> stateMachineList = new List<IStateMachine<StateDriverRunner>>();

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. 
		/// </summary>
		/// <typeparam name="TState">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <returns></returns>
		public StateMachine<TState> Initialize<TState>(MonoBehaviour component) where TState : struct, IConvertible, IComparable
		{
			var fsm = new StateMachine<TState>(component);

			stateMachineList.Add(fsm);

			return fsm;
		}

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
		/// </summary>
		/// <typeparam name="TState">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <param name="startState">The default start state</param>
		/// <returns></returns>
		public StateMachine<TState> Initialize<TState>(MonoBehaviour component, TState startState) where TState : struct, IConvertible, IComparable
		{
			var fsm = Initialize<TState>(component);

			fsm.ChangeState(startState);

			return fsm;
		}

		void FixedUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition && fsm.Component.enabled)
				{
						fsm.Driver.FixedUpdate.Invoke();
				}
			}
		}

		void Update()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition && fsm.Component.enabled)
				{
					fsm.Driver.Update.Invoke();
				}
			}
		}

		void LateUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition && fsm.Component.enabled)
				{
					fsm.Driver.LateUpdate.Invoke();
				}
			}
		}

		public static void DoNothing()
		{
		}

		public static IEnumerator DoNothingCoroutine()
		{
			yield break;
		}
	}
}