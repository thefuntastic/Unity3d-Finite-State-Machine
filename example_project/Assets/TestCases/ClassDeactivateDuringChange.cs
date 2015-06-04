using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

/// TEST DESCRIPTION
/// 
/// Test to make sure that when a game object reactivates itself after being set to inactive during a state transition, 
/// We don't stall when we try and call the next state. 
/// 
public class ClassDeactivateDuringChange : StateBehaviour 
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
	public int twoEnter;
	public int threeEnter;

	void Awake()
	{
		Initialize<States>();
		ChangeState(States.One);
	}

	
	IEnumerator One_Enter()
	{
		Debug.Log("One Enter " + Time.time);

		oneEnter++;

		yield return new WaitForSeconds(0.5f);

		ChangeState(States.Two);

		Debug.Log("One Complete " + Time.time);
	}

	IEnumerator One_Exit()
	{
		oneExit++;
		Debug.Log("One Exit " + Time.time);

		yield return new WaitForSeconds(0.5f); 

		helper.SetTimeout(ChangeToThree, 0.5f);
		gameObject.SetActive(false);

		oneExitComplete++;
		Debug.Log("One Exit Complete" + Time.time);
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
		ChangeState(States.Three);
	}
}
