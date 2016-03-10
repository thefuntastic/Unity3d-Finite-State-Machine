using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

/// TEST DESCRIPTION
/// 
/// Test to make sure that when a game object reactivates itself after being set to inactive during a state transition, 
/// We don't stall when we try and call the next state. 
/// 
public class ClassDeactivateDuringChange : MonoBehaviour 
{
	public enum States
	{
		One,
		Two,
		Three
	}

	public TimeoutHelper helper;

	public int oneEnter;
	public int oneExit;
	public int oneExitComplete;
	public int oneFinally;
	public int twoEnter;
	public int threeEnter;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
	}

	
	IEnumerator One_Enter()
	{
		Debug.Log("One Enter " + Time.time);

		oneEnter++;

		yield return new WaitForSeconds(0.5f);

		fsm.ChangeState(States.Two);

		Debug.Log("One Complete " + Time.time);
	}

	IEnumerator One_Exit()
	{
		oneExit++;
		Debug.Log("One Exit " + Time.time);

		yield return new WaitForSeconds(0.5f); 

		helper.SetTimeout(ChangeToThree, 0.5f);
		// This might be breaking the test runner. Need to try figure this out
		gameObject.SetActive(false);

		oneExitComplete++;
		Debug.Log("One Exit Complete" + Time.time);
	}

	void One_Finally()
	{
		oneFinally++;
	}

	void Two_Enter()
	{
		Debug.Log("Two Enter " + Time.time );
		twoEnter++;
	}

	void Three_Enter()
	{
		Debug.Log("Three Enter" + Time.time);
		threeEnter++;
	}

	public void ChangeToThree()
	{
		gameObject.SetActive(true);
		fsm.ChangeState(States.Three);
	}
}
