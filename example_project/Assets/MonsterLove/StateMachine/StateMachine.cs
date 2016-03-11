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
	}

	public interface IStateMachine
	{
		MonoBehaviour Component { get; }
		StateMapping CurrentStateMap { get; }
		bool IsInTransition { get; }
	}

	public class StateMachine<T> : IStateMachine where T : struct, IConvertible, IComparable
	{
		public event Action<T> Changed;

		private StateMachineRunner engine;
		private MonoBehaviour component;

		private StateMapping lastState;
		private StateMapping currentState;
		private StateMapping destinationState;

		private Dictionary<object, StateMapping> stateLookup;

		private readonly string[] ignoredNames = new[] { "add", "remove", "get", "set" };

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		public StateMachine(StateMachineRunner engine, MonoBehaviour component)
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
							targetState.hasEnterRoutine = true;
							targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							targetState.hasEnterRoutine = false;
							targetState.EnterCall = CreateDelegate<Action>(methods[i], component);
						}
						break;
					case "Exit":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.hasExitRoutine = true;
							targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(methods[i], component);
						}
						else
						{
							targetState.hasExitRoutine = false;
							targetState.ExitCall = CreateDelegate<Action>(methods[i], component);
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

			//Create nil state mapping
			currentState = new StateMapping(null);
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

					//Note: if we are currently in an EnterRoutine and Exit is also a routine, this will be skipped in ChangeToNewStateRoutine()
					break;
			}


			if ((currentState != null && currentState.hasExitRoutine) || nextState.hasEnterRoutine)
			{
				isInTransition = true;
				currentTransition = ChangeToNewStateRoutine(nextState, transition);
				engine.StartCoroutine(currentTransition);
			}
			else //Same frame transition, no coroutines are present
			{
				if (currentState != null)
				{
					currentState.ExitCall();
					currentState.Finally();
				}

				lastState = currentState;
				currentState = nextState;
				if (currentState != null)
				{
					currentState.EnterCall();
					if (Changed != null)
					{
						Changed((T) currentState.state);
					}
				}
				isInTransition = false;
			}
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping newState, StateTransition transition)
		{
			destinationState = newState; //Chache this so that we can overwrite it and hijack a transition

			if (currentState != null)
			{
				if (currentState.hasExitRoutine)
				{
					exitRoutine = currentState.ExitRoutine();

					if (exitRoutine != null && transition != StateTransition.Overwrite) //Don't wait for exit if we are overwriting
					{
						yield return engine.StartCoroutine(exitRoutine);
					}

					exitRoutine = null;
				}
				else
				{
					currentState.ExitCall();
				}

				currentState.Finally();
			}

			lastState = currentState;
			currentState = destinationState;

			if (currentState != null)
			{
				if (currentState.hasEnterRoutine)
				{
					enterRoutine = currentState.EnterRoutine();

					if (enterRoutine != null)
					{
						yield return engine.StartCoroutine(enterRoutine);
					}

					enterRoutine = null;
				}
				else
				{
					currentState.EnterCall();
				}

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

		public T LastState
		{
			get
			{
				if (lastState == null) return default(T);

				return (T) lastState.state;
			}
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
			get { return component; }
		}

		//Static Methods

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states.
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<T> Initialize(MonoBehaviour component)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<T>(component);
		}

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states. 
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <param name="startState">The default starting state</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<T> Initialize(MonoBehaviour component, T startState)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<T>(component, startState);
		}

	}

}
