using System;
using MonsterLove.StateMachine;
using UnityEngine;

public class ExampleAdvanced : MonoBehaviour
{
	public enum States
	{
		
	}

	public class Driver
	{
		
	}

	private StateMachine<States, Driver> fsm;

	private void Awake()
	{
		fsm = new StateMachine<States, Driver>(this);
	}
}