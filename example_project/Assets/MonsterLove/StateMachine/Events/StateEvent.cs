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