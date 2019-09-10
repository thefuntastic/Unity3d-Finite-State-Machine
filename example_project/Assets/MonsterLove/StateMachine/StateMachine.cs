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
using System.Linq;
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

	public interface IStateMachine<TDriver>
	{
		MonoBehaviour Component { get; }
		TDriver Driver { get; }
		bool IsInTransition { get; }
	}

	public class StateMachine<TState> : StateMachine<TState, StateMachineRunnerDriver> where TState : struct, IConvertible, IComparable
	{
		public StateMachine(MonoBehaviour component) : base(component)
		{
		}
	}

	public class StateMachine<TState, TDriver> : IStateMachine<TDriver> where TState : struct, IConvertible, IComparable where TDriver : class, new()
	{
		public event Action<TState> Changed;

		private MonoBehaviour component;

		private StateMapping<TState, TDriver> lastState;
		private StateMapping<TState, TDriver> currentState;
		private StateMapping<TState, TDriver> destinationState;
		private TDriver rootDriver;

		private Dictionary<object, StateMapping<TState, TDriver>> stateLookup;

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

#region Initialization

		public StateMachine(MonoBehaviour component)
		{
			this.component = component;

			//Define States
			var enumValues = Enum.GetValues(typeof(TState));
			if (enumValues.Length < 1)
			{
				throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition");
			}

			//Find all items in Driver class
			// public class Driver
			// {
			//     StateEvent Foo;        <- Selected
			//     StateEvent<int> Boo;   <- Selected 
			//     float x;               <- Throw exception
			// }
			List<FieldInfo> eventFields = GetFilteredFields(typeof(TDriver), "MonsterLove.StateMachine.StateEvent");
			Dictionary<string, FieldInfo> eventFieldsLookup = CreateFieldsLookup(eventFields);


			// Create a state mapping for each state defined in the enum
			stateLookup = CreateStateLookup(this, enumValues);

			// Ensures every field in the driver is initialised
			// eg mapping.Driver = new Driver(){
			//     StateEvent Foo = new StateEvent(mapping.TestInvokable)
			//     StateEvent Bar = new StateEvent(mapping.TestInvokable)
			// }
			foreach (var kvp in stateLookup)
			{
				var mapping = kvp.Value;
				mapping.driver = CreateDriver(null, eventFields);
				
			}

			//Collect methods in target component
			MethodInfo[] methods = component.GetType().GetMethods(bindingFlags);

			//Bind methods to states
			for (int i = 0; i < methods.Length; i++)
			{
				TState state;
				string evtName;
				if (!ParseName(methods[i], out state, out evtName))
				{
					continue; //Skip methods where State_Event name convention could not be parsed
				}

				StateMapping<TState, TDriver> mapping = stateLookup[state];

				if (eventFieldsLookup.ContainsKey(evtName))
				{
					//Bind methods defined in TDriver
					FieldInfo eventField = eventFieldsLookup[evtName];
					BindEvents(mapping, component, methods[i], eventField); //Todo, won't be necessary any more
				}
				else
				{
					//Bind Enter, Exit and Finally Methods
					BindEventsInternal(mapping, component, methods[i], evtName);
				}
			}

			//Create nil state mapping
			currentState = null; 
			
			//This is a head hurter: 
			//rootDriver.Foo<T> -> dispatcher -> __Mapping<States.One>.Driver.Foo.Invoke
			//						  			 \_Mapping<States.Two>.Driver.Foo.Invoke
			//									 \_Mapping<States.Three>.Driver.Foo.Invoke
			// 
			// basically we create and bind a dispatcher to our permanent root driver. This then routes events to each state mapping
			// having a fixed permanent driver means client code always has a valid reference
			rootDriver = CreateDriver(IsDispatchAllowed, eventFields);
			for (int i = 0; i < eventFields.Count; i++)
			{
				var driverEventDef = eventFields[i];
				BindDispatcher(stateLookup, rootDriver, driverEventDef, GetState);
			}
		}

		static List<FieldInfo> GetFilteredFields(Type type, string searchTerm)
		{
			List<FieldInfo> list = new List<FieldInfo>();

			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo item = fields[i];
				if (item.FieldType.ToString().Contains(searchTerm))
				{
					list.Add(item);
				}
				else
				{
					throw new ArgumentException(string.Format("{0} contains unsupported type {1}", type, item.FieldType));
				}
			}

			return list;
		}

		static Dictionary<string, FieldInfo> CreateFieldsLookup(List<FieldInfo> fields)
		{
			var dict = new Dictionary<string, FieldInfo>();

			for (int i = 0; i < fields.Count; i++)
			{
				FieldInfo item = fields[i];

				dict.Add(item.Name, item);
			}

			return dict;
		}

		static Dictionary<object, StateMapping<TState, TDriver>> CreateStateLookup(StateMachine<TState, TDriver> fsm, Array values)
		{
			var stateLookup = new Dictionary<object, StateMapping<TState, TDriver>>();
			for (int i = 0; i < values.Length; i++)
			{
				var mapping = new StateMapping<TState, TDriver>(fsm, (TState) values.GetValue(i));
				stateLookup.Add(mapping.state, mapping);
			}

			return stateLookup;
		}

		static TDriver CreateDriver(Func<bool> callback, List<FieldInfo> fieldInfos)
		{
			if (fieldInfos == null)
			{
				throw new ArgumentException(string.Format("Arguments cannot be null. Callback {0} fieldInfos {1}", callback, fieldInfos));
			}

			TDriver driver = new TDriver();

			for (int i = 0; i < fieldInfos.Count; i++)
			{
				//driver.Event = new StateEvent(callback)
				FieldInfo fieldInfo = fieldInfos[i]; //Event
				ConstructorInfo constructorInfo = fieldInfo.FieldType.GetConstructor(new Type[] {typeof(Func<bool>)}); //StateEvent(Func<Bool> testCallback)
				object obj = constructorInfo.Invoke(new object[] {callback}); //obj = new StateEvent(callback);
				fieldInfo.SetValue(driver, obj); //driver.Event = obj;
			}

			return driver;
		}

		static void BindDispatcher(Dictionary<object, StateMapping<TState, TDriver>> stateLookup, TDriver broadcaster, FieldInfo driverEvtDef, Func<TState> stateProvider)
		{
			var genericTypes = driverEvtDef.FieldType.GetGenericArguments();
			var actionType = GetActionType(genericTypes);
			
			var dispatcherType = GetDispatcherType(genericTypes);
			var constructorInfo = dispatcherType.GetConstructor(new Type[]{}); // StateDispatcher()
			var stateProviderInfo = dispatcherType.GetField("stateProvider", bindingFlags); 
			var dispatcherInvokeInfo = dispatcherType.GetMethod("Invoke"); //StateDispatcher.Invoke()
			var listenerLookupInfo = dispatcherType.GetField("listenerLookup", bindingFlags);
			var listenerLookupAddMethodInfo = listenerLookupInfo.FieldType.GetMethod("Add"); //Dictionary<TState, Action<TN>>.Add

			var eventInvokeInfo = driverEvtDef.FieldType.GetMethod("Invoke"); //StateEvent.Invoke();

			var dispatcher = constructorInfo.Invoke(new object[] { }); // dispatcher = new StateDispatcher()
			
			stateProviderInfo.SetValue(dispatcher, stateProvider); // dispatcher.stateProvider = (Func<TState>) stateProvider
			
			foreach(var kvp in stateLookup)
			{
				var stateMapping = kvp.Value;

				var listenerLookup = listenerLookupInfo.GetValue(dispatcher); // var listenerLookup = dispatcher.listenerLookup
				var stateEvent = driverEvtDef.GetValue(stateMapping.driver); //var stateEvent = stateMapping.Driver.Foo;
				var listener = Delegate.CreateDelegate(actionType, stateEvent, eventInvokeInfo); // (delegate) stateMapping.Driver.Foo.Invoke
				
				listenerLookupAddMethodInfo.Invoke(listenerLookup, new[] {kvp.Key, listener}); // listenerLookup.Add(state, stateMapping.Driver.Foo.Invoke)
			}
			
			var addListenerInfo = driverEvtDef.FieldType.GetMethod("AddListener"); // driver.Foo.AddListener
			var broadcasterStateEvent = driverEvtDef.GetValue(broadcaster); //var evt = driver.Foo;
			Delegate del = Delegate.CreateDelegate(actionType, dispatcher, dispatcherInvokeInfo); // (delegate) dispatcher.Invoke
			addListenerInfo.Invoke(broadcasterStateEvent, new object[] {del}); //driver.Foo.AddListener(dispatcher.Invoke)
		}

		static bool ParseName(MethodInfo methodInfo, out TState state, out string eventName)
		{
			state = default(TState);
			eventName = null;

			if (methodInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
			{
				return false;
			}

			string name = methodInfo.Name;
			int index = name.IndexOf('_');

			//Ignore functions without an underscore
			if (index < 0)
			{
				return false;
			}

			string stateName = name.Substring(0, index);
			eventName = name.Substring(index+1);

			try
			{
				state = (TState) Enum.Parse(typeof(TState), stateName);
			}
			catch (ArgumentException)
			{
				//Not an method as listed in the state enum
				return false;
			}

			return true;
		}

		static void BindEvents(StateMapping<TState, TDriver> targetState, Component component, MethodInfo stateTargetDef, FieldInfo driverEvtDef)
		{
			//evt.AddListener(State_Method); 
			var obj = driverEvtDef.GetValue(targetState.driver); //driver.Foo
			var addMethodInfo = driverEvtDef.FieldType.GetMethod("AddListener"); // driver.Foo.AddListener
			
			var genericTypes = driverEvtDef.FieldType.GetGenericArguments(); //get T1,T2,...TN from StateEvent<T1,T2,...TN>
			var actionType = GetActionType(genericTypes); //typeof(Action<T1,T2,...TN>)

			Delegate del = Delegate.CreateDelegate(actionType, component, stateTargetDef);
			addMethodInfo.Invoke(obj, new object[] {del}); //driver.Foo.AddListener(component.State_Event);
		}

		static void BindEventsInternal(StateMapping<TState, TDriver> targetState, Component component, MethodInfo method, string evtName)
		{
			switch (evtName)
			{
				case "Enter":
					if (method.ReturnType == typeof(IEnumerator))
					{
						targetState.hasEnterRoutine = true;
						targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
					}
					else
					{
						targetState.hasEnterRoutine = false;
						targetState.EnterCall = CreateDelegate<Action>(method, component);
					}

					break;
				case "Exit":
					if (method.ReturnType == typeof(IEnumerator))
					{
						targetState.hasExitRoutine = true;
						targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
					}
					else
					{
						targetState.hasExitRoutine = false;
						targetState.ExitCall = CreateDelegate<Action>(method, component);
					}

					break;
				case "Finally":
					targetState.Finally = CreateDelegate<Action>(method, component);
					break;
			}
		}

		static V CreateDelegate<V>(MethodInfo method, Object target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

			if (ret == null)
			{
				throw new ArgumentException("Unable to create delegate for method called " + method.Name);
			}

			return ret;
		}

		static Type GetDispatcherType(Type[] genericArgs)
		{
			Type[] concatenatedArgs = new Type[] {typeof(TState)};
			concatenatedArgs = concatenatedArgs.Concat(genericArgs).ToArray();
			switch (concatenatedArgs.Length)
			{
				case 1:
					return typeof(StateDispatcher<>).MakeGenericType(concatenatedArgs);
				case 2:
					return typeof(StateDispatcher<,>).MakeGenericType(concatenatedArgs);
				case 3:
					return typeof(StateDispatcher<,,>).MakeGenericType(concatenatedArgs);
				default:
					throw new ArgumentOutOfRangeException(string.Format("Cannot create StateDispatcher Type with {0} type arguments", genericArgs.Length));
			}
		}

		static Type GetActionType(Type[] genericArgs)
		{
			switch (genericArgs.Length)
			{
				case 0:
					return typeof(Action);
				case 1:
					return typeof(Action<>).MakeGenericType(genericArgs);
				case 2:
					return typeof(Action<,>).MakeGenericType(genericArgs);
				default:
					throw new ArgumentOutOfRangeException(string.Format("Cannot create Action Type with {0} type arguments", genericArgs.Length));
			}
		}
#endregion

#region ChangeStates

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

		private IEnumerator ChangeToNewStateRoutine(StateMapping<TState, TDriver> newState, StateTransition transition)
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

		IEnumerator WaitForPreviousTransition(StateMapping<TState, TDriver> nextState)
		{
			while (isInTransition)
			{
				yield return null;
			}

			ChangeState((TState) nextState.state);
		}

#endregion

#region Properties & Helpers

		public bool LastStateExists
		{
			get { return lastState != null; }
		}

		public TState LastState
		{
			get
			{
				if (lastState == null)
				{
					throw new NullReferenceException("LastState cannot be accessed before ChangeState() has been called at least twice");
				}

				return (TState) lastState.state;
			}
		}

		public TState State
		{
			get
			{
				if (currentState == null)
				{
					throw new NullReferenceException("State cannot be accessed before ChangeState() has been called at least once");
				}
				
				return (TState) currentState.state;
			}
		}

		public bool IsInTransition
		{
			get { return isInTransition; }
		}

		public TDriver Driver
		{
			get { return rootDriver; }
		}

		public MonoBehaviour Component
		{
			get { return component; }
		}

		//format as method so can be passed as Func<TState>
		private TState GetState()
		{
			return State;
		}
		
		private bool IsDispatchAllowed()
		{
			if (currentState == null)
			{
				return false;
			}
				
			if (IsInTransition)
			{
				return false;
			}

			return true;
		}
		

#endregion

#region Static API

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

#endregion
	}
}