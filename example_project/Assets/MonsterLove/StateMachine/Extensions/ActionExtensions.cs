using System;

namespace MonsterLove.StateMachine
{
    public static class Extensions
    {
        public static void Send(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        public static void Send<T>(this Action<T> action, T param)
        {
            if (action != null)
            {
                action(param);
            }
        }

        public static void Send<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            if (action != null)
            {
                action(param1, param2);
            }
        }

        public static void Send<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            if (action != null)
            {
                action(param1, param2, param3);
            }
        }

        public static void Send<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
            }
        }

        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15);
            }
        }
        
        public static void Send<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16)
        {
            if (action != null)
            {
                action(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
            }
        }
    }
}