using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Main : MonoBehaviour
{
	//Declare which states we'd like use
	public enum States
	{
		Init,
		Countdown,
		Play,
		Win,
		Lose
	}

	public float health = 100;
	public float damage = 20;

	private float startHealth;

	private StateMachine<States> fsm;

	private void Awake()
	{
		startHealth = health;

		//Initialize State Machine Engine		
		fsm = StateMachine<States>.Initialize(this, States.Init);
	}

	void OnGUI()
	{
		//Example of polling state 
		var state = fsm.State;

		GUILayout.BeginArea(new Rect(50,50,120,40));

		if(state == States.Init && GUILayout.Button("Start"))
		{
			fsm.ChangeState(States.Countdown);
		}
		if(state == States.Countdown)
		{
			GUILayout.Label("Look at Console");
		}
		if(state == States.Play)
		{
			if(GUILayout.Button("Force Win"))
			{
				fsm.ChangeState(States.Win);
			}
			
			GUILayout.Label("Health: " + Mathf.Round(health).ToString());
		}
		if(state == States.Win || state == States.Lose)
		{
			if(GUILayout.Button("Play Again"))
			{
				fsm.ChangeState(States.Countdown);
			}
		}

		GUILayout.EndArea();
	}

	private void Init_Enter()
	{
		Debug.Log("Waiting for start button to be pressed");
	}

	//We can return a coroutine, this is useful animations and the like
	private IEnumerator Countdown_Enter()
	{
		health = startHealth;

		Debug.Log("Starting in 3...");
		yield return new WaitForSeconds(0.5f);
		Debug.Log("Starting in 2...");
		yield return new WaitForSeconds(0.5f);
		Debug.Log("Starting in 1...");
		yield return new WaitForSeconds(0.5f);

		fsm.ChangeState(States.Play);

	}


	private void Play_Enter()
	{
		Debug.Log("FIGHT!");
	}

	private void Play_Update()
	{
		health -= damage * Time.deltaTime;
	
		if(health < 0)
		{
			fsm.ChangeState(States.Lose);
		}
	}

	void Play_Exit()
	{
		Debug.Log("Game Over");
	}

	void Lose_Enter()
	{
		Debug.Log("Lost");
	}

	void Win_Enter()
	{
		Debug.Log("Won");
	}
}
