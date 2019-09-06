/*
 * Copyright (c) 2016 Made With Monster Love (Pty) Ltd
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
using Random = UnityEngine.Random;

namespace MonsterLove.StateMachine
{
	public enum StateTransition
	{
		Safe,
		Overwrite,
	}

	public interface IStateMachine<TDriver>
	{
		MonoBehaviour Component { get; }
		TDriver Driver { get; }
		bool IsInTransition { get; }
	}

	public class StateMachine<TState> : StateMachine<TState, StateMachineDriverDefault> where TState : struct, IConvertible, IComparable
	{
		public StateMachine(MonoBehaviour component) : base(component)
		{
			
		}
	}
	
	public class StateMachine<TState, TDriver> : IStateMachine<TDriver> where TState : struct, IConvertible, IComparable where TDriver : class, new()
	{
		public event Action<TState> Changed;

		private MonoBehaviour component;

		private StateMapping<TDriver> lastState;
		private StateMapping<TDriver> currentState;
		private StateMapping<TDriver> destinationState;

		private Dictionary<object, StateMapping<TDriver>> stateLookup;

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		public StateMachine(MonoBehaviour component)
		{
			this.component = component;

			//Define States
			var values = Enum.GetValues(typeof(TState));
			if (values.Length < 1)
			{
				throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition");
			}

			stateLookup = new Dictionary<object, StateMapping<TDriver>>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping<TDriver>((Enum) values.GetValue(i));
				stateLookup.Add(mapping.state, mapping);
			}

			//Reflect methods
			MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
														 BindingFlags.NonPublic);

			Dictionary<string, FieldInfo> callbackDefintions = ReflectDriver(typeof(TDriver));

			//Bind methods to states
			var separator = "_".ToCharArray();
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
				{
					continue;
				}
				
				//TODO change this to index based lookup 
				//means instead of splitting all underscores, we only split the first one
				string[] names = methods[i].Name.Split(separator);

				//Ignore functions without an underscore
				if (names.Length <= 1)
				{
					continue;
				}

				string stateName = names[0];
				string eventName = names[1];
				
				Enum key;
				try
				{
					key = (Enum) Enum.Parse(typeof(TState), stateName);
				}
				catch (ArgumentException)
				{
					//Not an method as listed in the state enum
					continue;
				}

				StateMapping<TDriver> targetState = stateLookup[key];

				if (callbackDefintions.ContainsKey(eventName))
				{
					FieldInfo def = callbackDefintions[eventName];
					
					Delegate del = Delegate.CreateDelegate(def.FieldType, component, methods[i]);
					def.SetValue(targetState.driver, del);
					
					continue;
				}

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
				}
			}

			//Create nil state mapping
			currentState = new StateMapping<TDriver>(null);
		}


		static Dictionary<string, FieldInfo> ReflectDriver(Type driverType)
		{
			FieldInfo[] fields = driverType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			
			var dict = new Dictionary<string, FieldInfo>();

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo item = fields[i];
				if (item.FieldType.ToString().Contains("Action"))
				{
					dict.Add(item.Name, item);
				}
				// Debug.LogFormat("Name {0} type {1}", item.Name, item.FieldType);
			}
			
			return dict;
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

		public void ChangeState(TState newState)
		{
			ChangeState(newState, StateTransition.Safe);
		}

		public void ChangeState(TState newState, StateTransition transition)
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
				component.StopCoroutine(queuedChange);
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
							component.StartCoroutine(queuedChange);
							return;
						}
					}
					break;
				case StateTransition.Overwrite:
					if (currentTransition != null)
					{
						component.StopCoroutine(currentTransition);
					}
					if (exitRoutine != null)
					{
						component.StopCoroutine(exitRoutine);
					}
					if (enterRoutine != null)
					{
						component.StopCoroutine(enterRoutine);
					}

					//Note: if we are currently in an EnterRoutine and Exit is also a routine, this will be skipped in ChangeToNewStateRoutine()
					break;
			}


			if ((currentState != null && currentState.hasExitRoutine) || nextState.hasEnterRoutine)
			{
				isInTransition = true;
				currentTransition = ChangeToNewStateRoutine(nextState, transition);
				component.StartCoroutine(currentTransition);
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
						Changed((TState) currentState.state);
					}
				}
				isInTransition = false;
			}
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping<TDriver> newState, StateTransition transition)
		{
			destinationState = newState; //Cache this so that we can overwrite it and hijack a transition

			if (currentState != null)
			{
				if (currentState.hasExitRoutine)
				{
					exitRoutine = currentState.ExitRoutine();

					if (exitRoutine != null && transition != StateTransition.Overwrite) //Don't wait for exit if we are overwriting
					{
						yield return component.StartCoroutine(exitRoutine);
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
						yield return component.StartCoroutine(enterRoutine);
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
					Changed((TState) currentState.state);
				}
			}

			isInTransition = false;
		}

		IEnumerator WaitForPreviousTransition(StateMapping<TDriver> nextState)
		{
			while (isInTransition)
			{
				yield return null;
			}

			ChangeState((TState) nextState.state);
		}

		public TState LastState
		{
			get
			{
				if (lastState == null) return default(TState);

				return (TState) lastState.state;
			}
		}

		public TState State
		{
			get { return (TState) currentState.state; }
		}

		public bool IsInTransition
		{
			get { return isInTransition; }
		}

		public TDriver Driver
		{
			get { return (TDriver) CurrentStateMap.driver; }
		}
		
		public StateMapping<TDriver> CurrentStateMap
		{
			get { return currentState; }
		}

		public MonoBehaviour Component
		{
			get { return component; }
		}

		//Static Methods

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as defined by the supplied Enum, and returns a stateMachine instance used to transition states.
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<TState> Initialize(MonoBehaviour component)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<TState>(component);
		}

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as defined by the supplied Enum, and returns a stateMachine instance used to transition states. 
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <param name="startState">The default starting state</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine<TState> Initialize(MonoBehaviour component, TState startState)
		{
			var engine = component.GetComponent<StateMachineRunner>();
			if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();

			return engine.Initialize<TState>(component, startState);
		}

	}
	
	public class StateMapping<TDriver> where TDriver : class, new()
	{
		public object state;
		public TDriver driver;

		public bool hasEnterRoutine;
		public Action EnterCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> EnterRoutine = StateMachineRunner.DoNothingCoroutine;

		public bool hasExitRoutine;
		public Action ExitCall = StateMachineRunner.DoNothing;
		public Func<IEnumerator> ExitRoutine = StateMachineRunner.DoNothingCoroutine;

		public Action Finally = StateMachineRunner.DoNothing;
		
		public StateMapping(object state)
		{
			this.state = state;
			driver = new TDriver();
		}

	}

}
