using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Main : StateMachineBehaviour
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

	private void Awake()
	{
		startHealth = health;

		//Initialize State Machine Engine
		Initialize<States>();

		//Change to our first state
		ChangeState(States.Init);
	}

	void OnGUI()
	{
		//Sometimes it is a better pattern to poll state rather than to push state in each Enter/Exit function
		var state = GetState();

		if (state == null) return;

		GUILayout.BeginArea(new Rect(50,50,80,40));

		if(state.Equals(States.Init) && GUILayout.Button("Start"))
		{
			ChangeState(States.Countdown);
		}
		if(state.Equals(States.Play))
		{
			if(GUILayout.Button("Force Win"))
			{
				ChangeState(States.Win);
			}
			
			GUILayout.Label("Health: " + Mathf.Round(health).ToString());
		}
		if(state.Equals(States.Win) || state.Equals(States.Lose))
		{
			if(GUILayout.Button("Play Again"))
			{
				ChangeState(States.Countdown);
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

		ChangeState(States.Play);

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
			ChangeState(States.Lose);
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
