using System.Reflection;
using UnityEngine;
using System.Collections;

public class GodClassNaive : MonoBehaviour
{

	public enum States
	{
		Init,
		Play,
		Win,
		Lose
	}

	private StateMachine fsm;

	void Awake()
	{
		//This should go
		fsm = new StateMachine();
		fsm.AddState(States.Init, Init_Enter, Init_Exit, null);
		fsm.AddState(States.Play, Play_Enter, Play_Exit, null);
		fsm.AddState(States.Win, Win_Enter, Win_Exit, null);
		fsm.AddState(States.Lose, Lose_Enter, Lose_Exit, null);
		fsm.ChangeState(States.Init);

		//Instance > non static members
		//Declared > exclude inherited memebers	
		System.Reflection.MethodInfo[] info = typeof (GodClassNaive).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);


		foreach (var item in info)
		{
			Debug.Log(item);
		}

	}

	void Update()
	{
		//This should go.
		fsm.UpdateStates();
	}

	private void Init_Enter()
	{
		Debug.Log("Hello World");

		StartCoroutine(Init_StartComplete());
	}

	private IEnumerator Init_StartComplete()
	{
		yield return new WaitForSeconds(2f);

		fsm.ChangeState(States.Play);
	}

	private void Init_Exit()
	{
	}

	private void Play_Enter()
	{
		Debug.Log("Playing");

	}

	private void Play_Exit()
	{
	}

	private void Win_Enter()
	{
	}

	private void Win_Exit()
	{
	}

	private void Lose_Enter()
	{
	}

	private void Lose_Exit()
	{
	}
}
