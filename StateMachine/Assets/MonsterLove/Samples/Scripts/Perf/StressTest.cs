
using System;
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.Profiling;

public class StressTest : MonoBehaviour
{
	enum States
	{
		State0,
		State1,
		State2, 
		State3, 
		State4,
		State5, 
		State6, 
		State7, 
		State8, 
		State9, 
		State10, 
		State11, 
		State12, 
		State13, 
		State14,
	}

	public class Driver
	{
		public StateEvent Update;
		public StateEvent<int> OnChanged;
	}

	private StateMachine<States, Driver> fsm;

	private int value = 0;
	
	public void Awake()
	{
		fsm = new StateMachine<States, Driver>(this);
		fsm.ChangeState(States.State1);
	}

	private void Update()
	{
		Profiler.BeginSample("Fsm_Invoke");
		for (int i = 0; i < 10000; i++)
		{
			fsm.Driver.Update.Invoke();
			fsm.Driver.OnChanged.Invoke(i);
		}
		Profiler.EndSample();
		
		Profiler.BeginSample("Fsm_Native");
		for (int i = 0; i < 10000; i++)
		{
			State1_Update();
			State1_OnChanged(i);
		}
		Profiler.EndSample();
	}

	void State1_Update()
	{
		value++;
	}

	void State1_OnChanged(int value)
	{
		this.value = value;
	}

	void State2_Update()
	{
		value++;
	}

	void State2_OnChanged(int value)
	{
		this.value = -value;
	}
}