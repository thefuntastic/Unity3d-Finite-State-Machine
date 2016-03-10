using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ClassChangeDuringLongExit : MonoBehaviour 
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
	public int oneFinally;
	public int twoEnter;

	private StateMachine<States> fsm;

	void Awake()
	{
		fsm = GetComponent<StateMachineRunner>().Initialize<States>(this, States.One);
	}

	private float oneStartTime;
	private float oneDuration = 1f;

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

		//This should not cause the fsm.ChangeState() from update to double fire
		yield return null;

		//This should not cause it to triple fire
		yield return null;

		Debug.Log("One Exit Complete " + Time.time);
	}

	void One_Finally()
	{
		oneFinally++;
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
