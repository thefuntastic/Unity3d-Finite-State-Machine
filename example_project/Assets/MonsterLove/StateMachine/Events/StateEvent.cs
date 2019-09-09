using System;
using System.Collections.Generic;

namespace MonsterLove.StateMachine
{
    public class StateEvent
    {
        private Action action;
        private Func<bool> isInvokableTest;
        private List<Action> calls = new List<Action>();
        
        public StateEvent(Func<bool> isInvokableTest)
        {
            this.isInvokableTest = isInvokableTest;
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
            if (isInvokableTest == null)
            {
                throw new ArgumentException(string.Format("{0}: isInvokableTest cannot be null. Aborting", this));
            }

            if (!isInvokableTest())
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
        private Func<bool> isInvokableTest;
        private Action<T> action;
        private List<Action<T>> calls = new List<Action<T>>();
        
        public StateEvent(Func<bool> isInvokableTest)
        {
            this.isInvokableTest = isInvokableTest;
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
            if (isInvokableTest == null)
            {
                throw new ArgumentException(string.Format("{0}: isInvokableTest cannot be null. Aborting", this));
            }

            if (!isInvokableTest())
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
        private Func<bool> isInvokableTest;
        private Action<T1,T2> action;
        private List<Action<T1, T2>> calls = new List<Action<T1, T2>>();
        
        public StateEvent(Func<bool> isInvokableTest)
        {
            this.isInvokableTest = isInvokableTest;
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
            if (isInvokableTest == null)
            {
                throw new ArgumentException(string.Format("{0}: isInvokableTest cannot be null. Aborting", this));
            }

            if (!isInvokableTest())
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