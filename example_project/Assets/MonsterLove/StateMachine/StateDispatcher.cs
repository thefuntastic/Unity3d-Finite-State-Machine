using System;
using System.Collections.Generic;
#pragma warning disable 649 //stateProvider is set via reflection

namespace MonsterLove.StateMachine
{
	internal class StateDispatcher<TState>
	{
		private Func<TState> stateProvider;
		private Dictionary<TState, Action> listenerLookup = new Dictionary<TState, Action>();

		public void AddListener(TState state, Action listener)
		{
			listenerLookup.Add(state, listener);
		}
		
		public void Invoke()
		{
			TState state = stateProvider();

			var call = listenerLookup[state];

			call();
		}
	}
	
	public class StateDispatcher<TState, T>
	{
		private Func<TState> stateProvider;
		private Dictionary<TState, Action<T>> listenerLookup = new Dictionary<TState, Action<T>>();

		public void AddListener(TState state, Action<T> listener)
		{
			listenerLookup.Add(state, listener);
		}
		
		public void Invoke(T param)
		{
			TState state = stateProvider();

			var call = listenerLookup[state];

			call(param);
		}
	}
	
	public class StateDispatcher<TState, T1, T2>
	{
		private Func<TState> stateProvider;
		private Dictionary<TState, Action<T1,T2>> listenerLookup = new Dictionary<TState, Action<T1, T2>>();

		public void AddListener(TState state, Action<T1,T2> listener)
		{
			listenerLookup.Add(state, listener);
		}
		
		public void Invoke(T1 param1, T2 param2)
		{
			TState state = stateProvider();

			var call = listenerLookup[state];

			call(param1, param2);
		}
	}
}