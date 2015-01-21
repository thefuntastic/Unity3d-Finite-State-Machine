using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassChangeDuringUpdate : StateMachineBehaviour
{
	public enum States
	{
		NotUsed,
		AlsoNotUsed,
		One,
		Two
	}

	public float prizeIdleDuration;

	private float prizeIdleStartTime;

	public int oneEnter = 0;
	public int twoEnter = 0;

	

	void Awake()
	{
			
		Initialize<States>();
		

		ChangeState(States.One);
	}



	//Use timer here in stead of couroutines to prevent the stack depth getting too deeps, as these couroutines will cycle into each other
	void One_Enter()
	{
		Debug.Log("One: Entered " + Time.time);

		prizeIdleStartTime = Time.time;

		oneEnter++;
	}

	void One_Update()
	{
		if(Time.time - prizeIdleStartTime > prizeIdleDuration)
		{
			Debug.Log("Changing to Two : " + Time.time);
			ChangeState(States.Two);
		}
	}


	IEnumerator Two_Enter()
	{
		Debug.Log("Two Entered " + Time.time  );
		twoEnter++;

		yield return null;

	}



	
}
