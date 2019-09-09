using System;
using MonsterLove.StateMachine;

namespace MonsterLove.StateMachine
{
    public class StateMachineDriverDefault : StateMachineDriver
    {
        public StateEvent FixedUpdate;
        public StateEvent Update;
        public StateEvent LateUpdate;
    }
}
