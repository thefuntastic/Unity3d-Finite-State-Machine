using System;

namespace MonsterLove.StateMachine
{
    public class StateMachineDriverDefault : StateMachineDriver
    {
        public Action FixedUpdate;
        public Action Update;
        public Action LateUpdate;
    }
}
