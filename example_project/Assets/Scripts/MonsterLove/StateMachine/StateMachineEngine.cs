using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonsterLove.StateMachine
{
	public class StateMachineEngine : MonoBehaviour
	{
		private StateMapping currentState;

		private Dictionary<Enum, StateMapping> stateLookup;
		private Dictionary<string, Delegate> methodLookup; 

		public void Initialize<T, U>(T entity) where T : StateMachineBehaviour
		{
			//Define States
			var values = Enum.GetValues(typeof(U));
			stateLookup = new Dictionary<Enum, StateMapping>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping();
				stateLookup.Add((Enum) values.GetValue(i), mapping);
			}

			//Reflect methods
			var methods =
				typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public |
									  BindingFlags.NonPublic);

			//Bind methods to states
			var separator = "_".ToCharArray();
			for (int i = 0; i < methods.Length; i++)
			{
				var names = methods[i].Name.Split(separator);

				//Ignore functions without an underscore
				if(names.Length <= 1)
				{
					continue;
				}

				Enum key;
				try
				{
					key = (Enum) Enum.Parse(typeof(U), names[0]);
				}
				catch (ArgumentException)
				{
					Debug.LogWarning("Method with name " + methods[i].Name + " could not resolve a matching state. Check method spelling");
					continue;
				}

				var targetState = stateLookup[key];

				switch(names[1])
				{
					//case "Enter":
					//	targetState.Enter = CreateDelegate<T, Action>(methods[i], entity);
					//	break;
					//case "Exit":
					//	targetState.Exit = CreateDelegate<T, Action>(methods[i], entity);
					//	break;
					case "Update":
						targetState.Update = CreateDelegate<T, Action> (methods[i], entity);
						break;
					case "LateUpdate":
						targetState.LateUpdate = CreateDelegate<T, Action>(methods[i], entity);
						break;
					case "FixedUpdate":
						targetState.FixedUpdate = CreateDelegate<T, Action>(methods[i], entity);
						break;
				}
			}
		}

		private V CreateDelegate<T, V>(MethodInfo method, T target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof (V), target, method) as V);

			if(ret == null)
			{
				throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
			}
			return ret;
		}

		public void ChangeState(Enum newState)
		{
			if(stateLookup == null)
			{
				throw new Exception("States have not been configured, please call initialized before trying to set state");
			}

			if(!stateLookup.ContainsKey(newState))
			{
				throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
			}

			currentState = stateLookup[newState];
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
			if(currentState != null)
			{
				currentState.Update();
			}
		}

		void LateUpdate()
		{
			if (currentState != null)
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
	}

	public class StateMapping
	{
		public Enum state;

		//public Func<IEnumerator> Enter = StateMachineEngine.DoNothingCoroutine;
//		public Func<IEnumerator> Exit = StateMachineEngine.DoNothingCoroutine;
		public Action Enter = StateMachineEngine.DoNothing;
		public Action Exit = StateMachineEngine.DoNothing;
		public Action Update = StateMachineEngine.DoNothing;
		public Action LateUpdate = StateMachineEngine.DoNothing;
		public Action FixedUpdate = StateMachineEngine.DoNothing;

	}
}


