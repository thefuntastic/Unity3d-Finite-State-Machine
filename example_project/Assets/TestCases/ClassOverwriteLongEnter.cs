using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

/// TEST DESCRIPTION
/// 
/// Make sure that when we call overwrite during a long enter routine, Enter is cancelled and the next state is called immediately
/// without calling the exit function.
public class ClassOverwriteLongEnter : StateBehaviour 
{
	public enum States
	{
		One,
		Two,
		Three
	}

	public float oneDuration = 1f;

	public int oneEnter;
	public int oneUpdate;
	public int oneExit;
	public int twoEnter;
	public bool oneEntered = false;
	public bool twoEntered = true;

	private float oneStartTime;

	void Awake()
	{
		Initialize<States>();
		ChangeState(States.One);
	}

	IEnumerator One_Enter()
	{
		Debug.Log("One Enter " + Time.time);

		oneStartTime = Time.time;
		
		oneEnter++;

		yield return new WaitForSeconds(oneDuration * 2);

		Debug.Log("One Enter Complete " + Time.time);

		oneEntered = true;
	}

	//Use the mono behaviour update function to change our states outside the state convention 
	void Update()
	{
		var state = (States) GetState();

		if (state == States.One)
		{
			if (Time.time - oneStartTime > oneDuration)
			{
				Debug.Log("Changing to Two : " + Time.time);
				ChangeState(States.Two, StateTransition.Overwrite);
			}
		}
	}

	void One_Exit()
	{
		oneExit++;
		Debug.Log("One Exit " + Time.time);

		//if(!oneEntered)
		//{
		//	throw new Exception("One exit started before enter is complete");
		//}

	}

	IEnumerator Two_Enter()
	{
		Debug.Log("Two Enter " + Time.time );
		twoEnter++;

		yield return new WaitForSeconds(oneDuration*0.5f);

		Debug.Log("Two Enter Complete " + Time.time);
		twoEntered = true;
	}
}
