using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// TEST DESCRIPTION
/// 
/// Make sure that when we change during a monobehvaiour function call (ie Update) that we aren't double triggering changes to other states. Two_Enter is called once and only once.
public class ClassChangeDuringMonoUpdate : MonoBehaviour
{
	public enum States
	{
		NotUsed,
		AlsoNotUsed,
		One,
		Two
	}

	public float oneDuration = 1f;

	private float oneStartTime;

	public int oneEnter = 0;
	public int twoEnter = 0;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
	}
	
	//Unverified assumption: Use timer here in stead of couroutines to prevent the stack depth getting too deeps, as these couroutines will cycle into each other
	void One_Enter()
	{
		Debug.Log("One: Entered " + Time.time);

		oneStartTime = Time.time;

		oneEnter++;
	}

	//Use the mono behaviour update function to change our states. 
	void Update()
	{
		var state = fsm.State;

		if(state == States.One)
		{
			if (Time.time - oneStartTime > oneDuration)
			{
				Debug.Log("Changing to Two : " + Time.time);
				fsm.ChangeState(States.Two);
			}
		}
	}

	IEnumerator Two_Enter()
	{
		Debug.Log("Two Entered " + Time.time  );
		twoEnter++;

		yield return null;
	}
}
