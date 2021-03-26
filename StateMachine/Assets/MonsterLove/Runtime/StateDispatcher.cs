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