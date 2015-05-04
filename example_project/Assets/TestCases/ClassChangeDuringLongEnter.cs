using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ClassChangeDuringLongEnter : StateBehaviour 
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

	private float oneStartTime;
	private bool oneEntered = false;

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

		int count = 1;
		while (count++ < 120) // Two secs 
		{
			yield return null;
		}

		Debug.Log("One Complete " + Time.time);

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
				ChangeState(States.Two);
			}
		}
	}

	void One_Exit()
	{
		oneExit++;
		Debug.Log("One Exit " + Time.time);

		if(!oneEntered)
		{
			throw new Exception("One exit started before enter is complete");
		}

	}

	void Two_Enter()
	{
		Debug.Log("Two Enter " + Time.time );
		twoEnter++;
	}
}
