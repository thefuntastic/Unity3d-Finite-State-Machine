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

// Warning! 
// This is somewhat fragile Event pattern implementation. Recommended they aren't used outside of the state machine
// 
namespace MonsterLove.StateMachine
{
    public class StateEvent
    {
        private Func<bool> isInvokeAllowed;
        private List<Action> calls = new List<Action>();
        
        public StateEvent(Func<bool> isInvokeAllowed)
        {
            this.isInvokeAllowed = isInvokeAllowed;
        }

        public void AddListener(Action listener)
        {
            calls.Add(listener);
        }

        public void RemoveListener(Action listener)
        {
            calls.Remove(listener);
        }

        public void RemoveAllListeners()
        {
            calls.Clear();
        }

        public void Invoke()
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }
            
            //Susceptible to removal during iteration
            for (int i = 0; i < calls.Count; i++)
            {
                var call = calls[i];
                if(call != null)
                {
                    call();
                }
            }
        }
    }
    
    public class StateEvent<T>
    {
        private Func<bool> isInvokeAllowed;
        private List<Action<T>> calls = new List<Action<T>>();
        
        public StateEvent(Func<bool> isInvokeAllowed)
        {
            this.isInvokeAllowed = isInvokeAllowed;
        }

        public void AddListener(Action<T> listener)
        {
            calls.Add(listener);
        }

        public void RemoveListener(Action<T> listener)
        {
            calls.Remove(listener);
        }

        public void RemoveAllListeners()
        {
            calls.Clear();
        }

        public void Invoke(T param)
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }
            
            //Susceptible to removal during iteration
            for (int i = 0; i < calls.Count; i++)
            {
                var call = calls[i];
                if(call != null)
                {
                    call(param);
                }
            }
        }
    }
    
    public class StateEvent<T1, T2>
    {
        private Func<bool> isInvokeAllowed;
        private List<Action<T1, T2>> calls = new List<Action<T1, T2>>();
        
        public StateEvent(Func<bool> isInvokeAllowed)
        {
            this.isInvokeAllowed = isInvokeAllowed;
        }

        public void AddListener(Action<T1, T2> listener)
        {
            calls.Add(listener);
        }

        public void RemoveListener(Action<T1, T2> listener)
        {
            calls.Remove(listener);
        }

        public void RemoveAllListeners()
        {
            calls.Clear();
        }

        public void Invoke(T1 param1, T2 param2)
        {
            if (isInvokeAllowed != null && !isInvokeAllowed())
            {
                return;
            }
            
            //Susceptible to removal during iteration
            for (int i = 0; i < calls.Count; i++)
            {
                var call = calls[i];
                if(call != null)
                {
                    call(param1, param2);
                }
            }
        }
    }
}