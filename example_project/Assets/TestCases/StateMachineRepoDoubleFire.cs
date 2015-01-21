using System;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StateMachineRepoDoubleFire : StateMachineBehaviour
{
	public enum States
	{
		Idle,
		Prize,
		PrizeIdle,
		PrizeAttract
	}

	public float prizeIdleDuration;

	private float prizeIdleStartTime;

	public int oneEnter = 0;
	public int twoEnter = 0;

	

	void Awake()
	{
			
		Initialize<States>();
		

		ChangeState(States.PrizeIdle);
	}



	//Use timer here in stead of couroutines to prevent the stack depth getting too deeps, as these couroutines will cycle into each other
	void PrizeIdle_Enter()
	{
		Debug.Log("Prize idle: Entered " + Time.time);

		prizeIdleStartTime = Time.time;

		oneEnter++;
	}

	void PrizeIdle_Update()
	{
		if(Time.time - prizeIdleStartTime > prizeIdleDuration)
		{
			Debug.Log("Changing to attract : " + Time.time);
			ChangeState(States.PrizeAttract);
		}
	}


	IEnumerator PrizeAttract_Enter()
	{
		Debug.Log("Prize Attract: Entered " + Time.time  );
		twoEnter++;

		yield return null;


		//ChangeState(States.PrizeIdle);
	}



	
}
