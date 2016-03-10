using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ClassChangeFromLongExit : MonoBehaviour 
{
	public enum States
	{
		One,
		Two,
		Three
	}

	public int oneEnter;
	public int oneUpdate;
	public int oneExit;
	public int twoEnter;
	public bool exitComplete;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
	}

	private float oneStartTime;
	private float oneDuration = 1f;

	void One_Enter()
	{
		Debug.Log("One Enter " + Time.time);

		oneStartTime = Time.time;
		
		oneEnter++;
	}

	void One_Update()
	{
		oneUpdate++;

		if(Time.time - oneStartTime > oneDuration)
		{
			Debug.Log("Changing to two " + Time.time);
			fsm.ChangeState(States.Two);
		}
		
	}

	IEnumerator One_Exit()
	{
		oneExit++;
		Debug.Log("One Exit " + Time.time);

		StartCoroutine(DelayedChange());

		int count = 1;
		while (count++ < 120) // Two secs 
		{
			yield return null;
		}

		exitComplete = true;
		Debug.Log("One Exit Complete " + Time.time);
	}

	//Mimic change from external source while we happen to be exiting
	IEnumerator DelayedChange()
	{
		yield return new WaitForSeconds(oneDuration);
		fsm.ChangeState(States.Two);
	}

	IEnumerator Two_Enter()
	{
		Debug.Log("Two Enter " + Time.time );
		twoEnter++;

		int count = 0;
		while(count++ < 120) //2 secs
		{
			yield return null;
		}

		Debug.Log("Two Enter Complete " + Time.time);
	}
}
