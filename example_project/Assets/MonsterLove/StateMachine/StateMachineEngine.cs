/*
 * Copyright (c) 2012 Made With Mosnter Love (Pty) Ltd
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
using UnityEngine;
using Object = System.Object;

namespace MonsterLove.StateMachine
{
	public class StateMachineEngine : MonoBehaviour
	{
		private StateMapping currentState;

		private Dictionary<Enum, StateMapping> stateLookup;
		private Dictionary<string, Delegate> methodLookup;

		private readonly string[] ignoredNames = new[] { "add", "remove", "get", "set" };

		private bool isInTransition = false;

		public void Initialize<T>(StateMachineBehaviour entity)
		{
			//Define States
			var values = Enum.GetValues(typeof(T));
			stateLookup = new Dictionary<Enum, StateMapping>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping((Enum) values.GetValue(i));
				stateLookup.Add(mapping.state, mapping);
			}

			//Reflect methods
			var methods = entity.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
									  BindingFlags.NonPublic);

			//Bind methods to states
			var separator = "_".ToCharArray();
			for (int i = 0; i < methods.Length; i++)
			{
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
					//Some things (evetns, properties) generate automatic method. Ignore these
					for (int j = 0; j < ignoredNames.Length; j++)
					{
						if (names[0] == ignoredNames[j])
						{
							goto SkipWarning;
						}
					}

					Debug.LogWarning("Method with name " + methods[i].Name + " could not resolve a matching state. Check method spelling");
					continue;

				SkipWarning:
					continue;
				}

				var targetState = stateLookup[key];

				switch (names[1])
				{
					case "Enter":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.Enter = CreateDelegate<Func<IEnumerator>>(methods[i], entity);
						}
						else
						{
							var action = CreateDelegate<Action>(methods[i], entity);
							targetState.Enter = () => { action(); return null; };
						}
						break;
					case "Exit":
						if (methods[i].ReturnType == typeof(IEnumerator))
						{
							targetState.Exit = CreateDelegate<Func<IEnumerator>>(methods[i], entity);
						}
						else
						{
							var action = CreateDelegate<Action>(methods[i], entity);
							targetState.Exit = () => { action(); return null; };
						}
						break;
					case "Update":
						targetState.Update = CreateDelegate<Action>(methods[i], entity);
						break;
					case "LateUpdate":
						targetState.LateUpdate = CreateDelegate<Action>(methods[i], entity);
						break;
					case "FixedUpdate":
						targetState.FixedUpdate = CreateDelegate<Action>(methods[i], entity);
						break;
				}
			}
		}

		private V CreateDelegate<V>(MethodInfo method, Object target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof (V), target, method) as V);

			if (ret == null)
			{
				throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
			}
			return ret;

		}

		public void ChangeState(Enum newState)
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

			StartCoroutine(ChangeToNewStateRoutine(nextState));
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping newState)
		{
			isInTransition = true;

			if (currentState != null)
			{

				var exitRoutine = currentState.Exit();

				if (exitRoutine != null)
				{
					yield return StartCoroutine(exitRoutine);
				}
			}

			currentState = newState;

			if (currentState != null)
			{
				var enterRoutine = currentState.Enter();

				if (enterRoutine != null)
				{
					yield return StartCoroutine(enterRoutine);
				}
			}

			isInTransition = false;
		}

		void FixedUpdate()
		{
			if (currentState != null)
			{
				currentState.FixedUpdate();
			}
		}

		void Update()
		{
			if (currentState != null && !isInTransition)
			{
				currentState.Update();
			}
		}

		void LateUpdate()
		{
			if (currentState != null && !isInTransition)
			{
				currentState.LateUpdate();
			}
		}

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

		public Enum GetState()
		{
			if (currentState != null)
			{
				return currentState.state;
			}

			return null;
		}
	}

	public class StateMapping
	{
		public Enum state;

		public Func<IEnumerator> Enter = StateMachineEngine.DoNothingCoroutine;
		public Func<IEnumerator> Exit = StateMachineEngine.DoNothingCoroutine;
		//public Action Enter = StateMachineEngine.DoNothing;
		//public Action Exit = StateMachineEngine.DoNothing;
		public Action Update = StateMachineEngine.DoNothing;
		public Action LateUpdate = StateMachineEngine.DoNothing;
		public Action FixedUpdate = StateMachineEngine.DoNothing;

		public StateMapping(Enum state)
		{
			this.state = state;
		}

	}
}


