using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

/// TEST DESCRIPTION
/// 
/// Test to make sure that when a game object reactivates itself after being set to inactive during a state transition, 
/// We don't stall when we try and call the next state. 
/// 
public class ClassDisabled : MonoBehaviour 
{
	public enum States
	{
		One,
		Two,
		Three
	}

	
	public int oneUpdate;
	public int twoUpdate;

	public TimeoutHelper helper;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
	}

	void One_Enter()
	{
		helper.SetTimeout(() => fsm.ChangeState(States.Two), 0.4f);
	}

	void One_Update()
	{
		oneUpdate++;
	
	}

	void Two_Enter()
	{
		enabled = false;

		helper.SetTimeout(() => fsm.ChangeState(States.Three), 0.4f);
	}

	void Two_Update()
	{
		twoUpdate++;
	}
}
