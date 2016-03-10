using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassChangeDuringUpdate : MonoBehaviour
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

	public StateMachine<States> fsm;

	void Awake()
	{
		fsm = StateMachine<States>.Initialize(this, States.One);
	}

	//Use timer here in stead of couroutines to prevent the stack depth getting too deeps, as these couroutines will cycle into each other
	void One_Enter()
	{
		Debug.Log("One: Entered " + Time.time);

		oneStartTime = Time.time;

		oneEnter++;
	}

	void One_Update()
	{
		if(Time.time - oneStartTime > oneDuration)
		{
			Debug.Log("Changing to Two : " + Time.time);
			fsm.ChangeState(States.Two);
		}
	}


	IEnumerator Two_Enter()
	{
		Debug.Log("Two Entered " + Time.time  );
		twoEnter++;

		yield return null;

	}



	
}
