using System;
using System.Collections;
using MonsterLove.StateMachine;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class TestCustomDriver
{
    public enum States
    {
        One,
        Two,
        Three,
        Four,
    }

    public class CustomDriver : StateMachineDriver
    {
        public Action Foo;
        public Action<int> Bar;
        public Action<int, int> Baz;
    }

    private GameObject go;
    private StateClass behaviour;
    private StateMachine<States, CustomDriver> fsm;

    [SetUp]
    public void Init()
    {
        go = new GameObject();
        behaviour = go.AddComponent<StateClass>();

        fsm = new StateMachine<States, CustomDriver>(behaviour);
    }

    [TearDown]
    public void Kill()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void TestCustomEvents()
    {
        fsm.ChangeState(States.One);

        fsm.Send(fsm.Driver.Foo);
        fsm.Send(fsm.Driver.Bar, 5);
        fsm.Send(fsm.Driver.Baz, 6, 7);

        Assert.AreEqual(1, behaviour.oneFoo);
        Assert.AreEqual(1, behaviour.oneBar);
        Assert.AreEqual(1, behaviour.oneBaz);
        Assert.AreEqual(5, behaviour.oneBarValue);
        Assert.AreEqual(6, behaviour.oneBazValueA);
        Assert.AreEqual(7, behaviour.oneBazValueB);

        Assert.AreEqual(0, behaviour.twoFoo);
        Assert.AreEqual(0, behaviour.twoBar);
        Assert.AreEqual(0, behaviour.twoBaz);
        Assert.AreEqual(0, behaviour.twoBarValue);
        Assert.AreEqual(0, behaviour.twoBazValueA);
        Assert.AreEqual(0, behaviour.twoBazValueB);

        fsm.ChangeState(States.Two);

        fsm.Send(fsm.Driver.Foo);
        fsm.Send(fsm.Driver.Bar, 8);
        fsm.Send(fsm.Driver.Baz, 9, 10);

        Assert.AreEqual(1, behaviour.oneFoo);
        Assert.AreEqual(1, behaviour.oneBar);
        Assert.AreEqual(1, behaviour.oneBaz);
        Assert.AreEqual(5, behaviour.oneBarValue);
        Assert.AreEqual(6, behaviour.oneBazValueA);
        Assert.AreEqual(7, behaviour.oneBazValueB);

        Assert.AreEqual(1, behaviour.twoFoo);
        Assert.AreEqual(1, behaviour.twoBar);
        Assert.AreEqual(1, behaviour.twoBaz);
        Assert.AreEqual(8, behaviour.twoBarValue);
        Assert.AreEqual(9, behaviour.twoBazValueA);
        Assert.AreEqual(10, behaviour.twoBazValueB);
    }

    private class StateClass : MonoBehaviour
    {
        public int oneEnter;
        public int oneFoo;
        public int oneBar;
        public int oneBaz;
        public int oneExit;

        public int oneBarValue;
        public int oneBazValueA;
        public int oneBazValueB;

        public int twoEnter;
        public int twoFoo;
        public int twoBar;
        public int twoBaz;
        public int twoExit;

        public int twoBarValue;
        public int twoBazValueA;
        public int twoBazValueB;

        void One_Enter()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "One Enter", Time.frameCount);
            oneEnter++;
        }

        void One_Foo()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "One Foo", Time.frameCount);
            oneFoo++;
        }

        void One_Bar(int value)
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "One Bar", Time.frameCount);
            oneBar++;
            oneBarValue = value;
        }

        void One_Baz(int valueA, int valueB)
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "One Baz", Time.frameCount);
            oneBaz++;
            oneBazValueA = valueA;
            oneBazValueB = valueB;
        }

        void One_Exit()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "One Exit", Time.frameCount);
            oneExit++;
        }

        void Two_Enter()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "Two Enter", Time.frameCount);
            twoEnter++;
        }

        void Two_Foo()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "Two Foo", Time.frameCount);
            twoFoo++;
        }

        void Two_Bar(int value)
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "Two Bar", Time.frameCount);
            twoBar++;
            twoBarValue = value;
        }

        void Two_Baz(int valueA, int valueB)
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "Two Baz", Time.frameCount);
            twoBaz++;
            twoBazValueA = valueA;
            twoBazValueB = valueB;
        }

        void Two_Exit()
        {
            //Debug.LogFormat("State:{0} Frame:{1}", "Two Exit", Time.frameCount);
            twoExit++;
        }
    }
}