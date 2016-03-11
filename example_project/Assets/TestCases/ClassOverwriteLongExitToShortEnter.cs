using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

/// TEST DESCRIPTION
/// 
/// What happens when IEnumerator Exit is overwritten by void Enter
public class ClassOverwriteLongExitToShortEnter : MonoBehaviour 
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
	public int oneFinally;
	public int twoEnter;
	public bool oneEntered = false;
	public bool oneExited = false;
	public bool twoEntered = true;

	private float oneStartTime;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
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
		var state = fsm.State;

		if (state == States.One)
		{
			if (Time.time - oneStartTime > oneDuration)
			{
				Debug.Log("Changing to Two : " + Time.time);
				fsm.ChangeState(States.Two, StateTransition.Overwrite);
			}
		}
	}

	IEnumerator One_Exit()
	{
		Debug.Log("One Exit " + Time.time);
		oneExit++;

		yield return new WaitForSeconds(oneDuration * 2);

		oneExited = true;
		Debug.Log("One Exit Complete" + Time.time);
	}


	void One_Finally()
	{
		oneFinally++;
		Debug.Log("One Finally " + Time.time);
	}

	void Two_Enter()
	{
		Debug.Log("Two Enter " + Time.time );
		twoEnter++;

		//yield return new WaitForSeconds(oneDuration*0.5f);
		//Debug.Log("Two Enter Complete " + Time.time);
		twoEntered = true;
	}
}
