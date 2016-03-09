/*
 * Copyright (c) 2016 Made With Mosnter Love (Pty) Ltd
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
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = System.Object;

namespace MonsterLove.StateMachine
{
	public enum StateTransition
	{
		Safe,
		Overwrite,
		//Blend,
	}

	public interface IStateMachine
	{
		StateMapping CurrentStateMap { get; }
		bool IsInTransition { get; }
	}

	public class StateEngine : MonoBehaviour
	{
		private List<IStateMachine> stateMachineList = new List<IStateMachine>();
		private Dictionary<IStateMachine, MonoBehaviour> stateMachineLookup = new Dictionary<IStateMachine, MonoBehaviour>();

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition to the defualt state
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(MonoBehaviour component) where T : struct, IConvertible, IComparable
		{
			return Initialize<T>(component, default(T));
		}

		/// <summary>
		/// Creates a stateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the startState
		/// </summary>
		/// <typeparam name="T">An Enum listing different state transitions</typeparam>
		/// <param name="component">The component whose state will be managed</param>
		/// <param name="startState">The default start state</param>
		/// <returns></returns>
		public StateMachine<T> Initialize<T>(MonoBehaviour component, T startState) where T : struct, IConvertible, IComparable
		{
			var fsm = new StateMachine<T>(this, component, startState);

			stateMachineLookup = new Dictionary<IStateMachine, MonoBehaviour>();
			stateMachineLookup.Add(fsm, component);

			stateMachineList.Add(fsm);

			return fsm;
		}

		void FixedUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				stateMachineList[i].CurrentStateMap.FixedUpdate();
			}
		}

		void Update()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition)
				{
					fsm.CurrentStateMap.Update();
				}
			}
		}

		void LateUpdate()
		{
			for (int i = 0; i < stateMachineList.Count; i++)
			{
				var fsm = stateMachineList[i];
				if (!fsm.IsInTransition)
				{
					fsm.CurrentStateMap.LateUpdate();
				}
			}
		}

		//void OnCollisionEnter(Collision collision)
		//{
		//	if(currentState != null && !IsInTransition)
		//	{
		//		currentState.OnCollisionEnter(collision);
		//	}
		//}

		public static void DoNothing()
		{
		}

		public static void DoNothingCollider(Collider other)
		{
		}

		public static void DoNothingCollision(Collision other)
		{
		}

		public static IEnumerator DoNothingCoroutine()
		{
			yield break;
		}
	}

	public class StateMachine<T> : IStateMachine where T : struct, IConvertible, IComparable
	{
		public event Action<T> Changed;

		private StateEngine engine;
		private MonoBehaviour component;

		private StateMapping currentState;
		private StateMapping destinationState;

		private Dictionary<object, StateMapping> stateLookup;
		private Dictionary<string, Delegate> methodLookup;

		private readonly string[] ignoredNames = new[] { "add", "remove", "get", "set" };

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		public StateMachine(StateEngine engine, MonoBehaviour component, T startState)
		{
			this.engine = engine;
			this.component = component;

			//Define States
			var values = Enum.GetValues(typeof(T));
			if (values.Length < 1) { throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition"); }

			stateLookup = new Dictionary<object, StateMapping>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping((Enum) values.GetValue(i));
				stateLookup.Add(mapping.state, mapping);
			}

			//Reflect methods
			var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
									  BindingFlags.NonPublic);

			//Bind methods to states
			var separator = "_".ToCharArray();
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
				{
					continue;
				}

				var names = methods[i].Name.Split(separator);

				//Ignore functions without an underscore
				if (names.Length <= 1)
				{
					continue;
				}

				Enum key;
				try
				{
					key = (Enum) Enum.Parse(typeof(T), names[0]);
				}
				catch (ArgumentException)
				{
					//Not an method as listed in the state enum
					continue;
				}

				var targetState = stateLookup[key];

				switch (names[1])
				{
					case "Enter":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.Enter = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							var action = CreateDelegate<Action>(methods[i], component);
							targetState.Enter = () => { action(); return null; };
						}
						break;
					case "Exit":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.Exit = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							var action = CreateDelegate<Action>(methods[i], component);
							targetState.Exit = () => { action(); return null; };
						}
						break;
					case "Finally":
						targetState.Finally = CreateDelegate<Action>(methods[i], component);
						break;
					case "Update":
						targetState.Update = CreateDelegate<Action>(methods[i], component);
						break;
					case "LateUpdate":
						targetState.LateUpdate = CreateDelegate<Action>(methods[i], component);
						break;
					case "FixedUpdate":
						targetState.FixedUpdate = CreateDelegate<Action>(methods[i], component);
						break;
					case "OnCollisionEnter":
						targetState.OnCollisionEnter = CreateDelegate<Action<Collision>>(methods[i], component);
						break;
				}
			}

			ChangeState(startState);
		}

		private V CreateDelegate<V>(MethodInfo method, Object target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

			if (ret == null)
			{
				throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
			}
			return ret;

		}

		public void ChangeState(T newState)
		{
			ChangeState(newState, StateTransition.Safe);
		}

		public void ChangeState(T newState, StateTransition transition)
		{
			if (stateLookup == null)
			{
				throw new Exception("States have not been configured, please call initialized before trying to set state");
			}

			if (!stateLookup.ContainsKey(newState))
			{
				throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
			}

			var nextState = stateLookup[newState];

			if (currentState == nextState) return;

			//Cancel any queued changes.
			if (queuedChange != null)
			{
				engine.StopCoroutine(queuedChange);
				queuedChange = null;
			}

			switch (transition)
			{
				//case StateMachineTransition.Blend:
				//Do nothing - allows the state transitions to overlap each other. This is a dumb idea, as previous state might trigger new changes. 
				//A better way would be to start the two couroutines at the same time. IE don't wait for exit before starting start.
				//How does this work in terms of overwrite?
				//Is there a way to make this safe, I don't think so? 
				//break;
				case StateTransition.Safe:
					if (isInTransition)
					{
						if (exitRoutine != null) //We are already exiting current state on our way to our previous target state
						{
							//Overwrite with our new target
							destinationState = nextState;
							return;
						}

						if (enterRoutine != null) //We are already entering our previous target state. Need to wait for that to finish and call the exit routine.
						{
							//Damn, I need to test this hard
							queuedChange = WaitForPreviousTransition(nextState);
							engine.StartCoroutine(queuedChange);
							return;
						}
					}
					break;
				case StateTransition.Overwrite:
					if (currentTransition != null)
					{
						engine.StopCoroutine(currentTransition);
					}
					if (exitRoutine != null)
					{
						engine.StopCoroutine(exitRoutine);
					}
					if (enterRoutine != null)
					{
						engine.StopCoroutine(enterRoutine);
					}

					if (currentState != null) currentState.Finally();

					currentState = null; //We need to set current state to null so that we don't trigger it's exit routine
					break;
			}

			isInTransition = true;
			currentTransition = ChangeToNewStateRoutine(nextState);
			engine.StartCoroutine(currentTransition);
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping newState)
		{
			destinationState = newState; //Chache this so that we can overwrite it and hijack a transition

			if (currentState != null)
			{
				exitRoutine = currentState.Exit();

				if (exitRoutine != null)
				{
					yield return engine.StartCoroutine(exitRoutine);
				}

				exitRoutine = null;

				currentState.Finally();
			}

			currentState = destinationState;

			if (currentState != null)
			{
				enterRoutine = currentState.Enter();

				if (enterRoutine != null)
				{
					yield return engine.StartCoroutine(enterRoutine);
				}

				enterRoutine = null;

				//Broadcast change only after enter transition has begun. 
				if (Changed != null)
				{
					Changed((T) currentState.state);
				}
			}

			isInTransition = false;
		}

		IEnumerator WaitForPreviousTransition(StateMapping nextState)
		{
			while (isInTransition)
			{
				yield return null;
			}

			ChangeState((T) nextState.state);
		}

		public T State
		{
			get { return (T) currentState.state; }
		}

		public bool IsInTransition
		{
			get { return isInTransition; }
		}

		public StateMapping CurrentStateMap
		{
			get { return currentState; }
		}

		public MonoBehaviour Component
		{
			get { return Component; }
		}
	}

	public class StateMapping
	{
		public object state;

		public Func<IEnumerator> Enter = StateEngine.DoNothingCoroutine;
		public Func<IEnumerator> Exit = StateEngine.DoNothingCoroutine;
		public Action Finally = StateEngine.DoNothing;
		public Action Update = StateEngine.DoNothing;
		public Action LateUpdate = StateEngine.DoNothing;
		public Action FixedUpdate = StateEngine.DoNothing;
		public Action<Collision> OnCollisionEnter = StateEngine.DoNothingCollision;

		public StateMapping(object state)
		{
			this.state = state;
		}

	}
}


