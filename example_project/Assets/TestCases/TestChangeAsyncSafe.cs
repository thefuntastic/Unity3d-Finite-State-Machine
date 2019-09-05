using System;
using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;
using Random = UnityEngine.Random;

namespace Tests
{
    public class TestChangeAsyncSafe
    {
        public enum States
        {
            One,
            Two,
            Three, 
            Four, 
        }
        
        private GameObject go;
        private StateClass behaviour;
        private StateMachine<States> fsm;

        [SetUp]
        public void Init()
        {
            go = new GameObject();
            behaviour = go.AddComponent<StateClass>();
            behaviour.longDuration = 1;
            
            fsm = StateMachine<States>.Initialize(behaviour);
        }
        
        [UnityTest]
        public IEnumerator TestAsyncEnterExit()
        {
            // 2
            
            fsm.ChangeState(States.Two, StateTransition.Safe);
            
            Assert.AreEqual(0, behaviour.twoExit);
            Assert.AreEqual(0, behaviour.oneEnter);
            
            // 2\__/1
            
            fsm.ChangeState(States.One, StateTransition.Safe);
            
            Assert.AreEqual(0, behaviour.twoExit);
            Assert.AreEqual(0, behaviour.oneEnter);
            
            yield return new WaitForSeconds(behaviour.longDuration * 2 + 0.1f);
            
            Assert.AreEqual(1, behaviour.twoExit);
            Assert.AreEqual(1, behaviour.oneEnter);
        }

        [UnityTest]
        public IEnumerator TestChangeDuringAsyncEnter()
        {
            // /1
            fsm.ChangeState(States.One, StateTransition.Safe);

            yield return new WaitForSeconds(behaviour.longDuration * 0.5f);
            
            Assert.AreEqual(0, behaviour.oneExit);
            Assert.AreEqual(0, behaviour.twoEnter);
            
            
            // /1__2
            fsm.ChangeState(States.Two);
            
            Assert.AreEqual(0, behaviour.oneExit);
            Assert.AreEqual(0, behaviour.twoEnter);

            yield return new WaitForSeconds(behaviour.longDuration + 0.1f); //Wait till after first transition
            
            Assert.AreEqual(1, behaviour.oneExit);
            Assert.AreEqual(1, behaviour.twoEnter);
        }
        
        [UnityTest]
        public IEnumerator TestChangeDuringAsyncExit()
        {
            // 2
            fsm.ChangeState(States.Two, StateTransition.Safe);
            
            // 2\__3 Start long exit to three
            fsm.ChangeState(States.Three, StateTransition.Safe);
            
            yield return new WaitForSeconds(behaviour.longDuration * 0.5f);

            Assert.AreEqual(0, behaviour.twoExit);
            Assert.AreEqual(0, behaviour.threeEnter);
            Assert.AreEqual(0, behaviour.threeExit);
            Assert.AreEqual(0, behaviour.fourEnter);
            
            // 2\__4
            fsm.ChangeState(States.Four); //Try interrupt long exit
            
            Assert.AreEqual(0, behaviour.twoExit);
            Assert.AreEqual(0, behaviour.threeEnter);
            Assert.AreEqual(0, behaviour.threeExit);
            Assert.AreEqual(0, behaviour.fourEnter);
            
            yield return new WaitForSeconds(behaviour.longDuration + 0.1f);
            
            //Ensure all states have been triggered
            Assert.AreEqual(1, behaviour.twoExit);
            Assert.AreEqual(0, behaviour.threeEnter); //Three never runs
            Assert.AreEqual(0, behaviour.threeExit);
            Assert.AreEqual(1, behaviour.fourEnter);
        }

        private class StateClass : MonoBehaviour 
        {
            public float longDuration;

            public int oneEnter;
            public int oneExit;
            public int twoEnter;
            public int twoExit;
            public int threeEnter;
            public int threeExit;
            public int fourEnter;
            public int fourExit;

            private bool oneEntered = false;
            
            IEnumerator One_Enter()
            {
                Debug.Log("One Enter " + Time.time);

                yield return new WaitForSeconds(longDuration);

                Debug.Log("One Enter Complete " + Time.time);
                
                oneEnter++;
                oneEntered = true;
            }
            
            void One_Exit()
            {
                oneExit++;
                Debug.Log("One Exit " + Time.time);

                if(!oneEntered)
                {
                    throw new Exception("One exit started before enter is complete");
                }

            }

            void Two_Enter()
            {
                Debug.Log("Two Enter " + Time.time );
                twoEnter++;
            }

            IEnumerator Two_Exit()
            {
                Debug.Log("Two Exit " + Time.time);

                yield return new WaitForSeconds(longDuration);

                Debug.Log("Two Exit Complete " + Time.time);

                twoExit++;
            }

            void Three_Enter()
            {
                Debug.Log("Three Enter");
                threeEnter++;
            }

            void Three_Exit()
            {
                Debug.Log("Three Exit");
                threeExit++;
            }

            void Four_Enter()
            {
                Debug.Log("Four Enter");
                fourEnter++;
            }

            void Four_Exit()
            {
                Debug.Log("Four Exit");
                fourExit++;
            }
        }
    }
}
