using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	
	//Declare enum for each of our states. This makes it easier to specify states in the inpsector
	public enum States 
	{
		Init,
		GamePlay,
		GameOverWinner,
		GameOverLoser,
	}
	
	//Refernce to a regular game object, in this case our not so heroic main player
	public Player player;
	public GUIManager gui;
	
	//By making these public properties, and using [System.Serializable] in the state classes, these will now appear in the inspector
	public StateInit stateInit = new StateInit();
	public StateGamePlay stateGamePlay = new StateGamePlay();
	public StateGameOverWinner stateGameOverWinner = new StateGameOverWinner();
	public StateGameOverLoser stateGameOverLoser = new StateGameOverLoser();
		
	//The referecne to our state machine
	private FiniteStateMachine<Main, Main.States> FSM;
	

	void Start () 
	{
		//Initialise the state machine
		FSM = new FiniteStateMachine<Main, Main.States>(this);
		FSM.RegisterState(stateInit);
		FSM.RegisterState(stateGamePlay);
		FSM.RegisterState(stateGameOverWinner);
		FSM.RegisterState(stateGameOverLoser);
		
		//Kick things off
		ChangeState(stateInit.StateID);
	}
	
	void Update () 
	{
		//Must remember to update our state machine
		FSM.Update();
	}
	
	//Some examples of utility functions for changing state.
	//Its up to you to decide how you actually change you state
	public void ChangeState(States newState)
	{
		FSM.ChangeState(newState);
	}
	
	public void ChangeState(States newState, float delay)
	{
		StartCoroutine(ChangeStateWithDelay(newState, delay));
	}
	
	private IEnumerator ChangeStateWithDelay(States newState, float delay)
	{
		yield return new WaitForSeconds(delay);
		ChangeState(newState);
	}
	
	//Its a wise idea to create methods for common actions that will be used accross states as its easier to keep track and make changes. 
	public void EnablePlayer(bool isEnabled)
	{
		//player.enabled = isEnabled;
		player.gameObject.SetActiveRecursively(isEnabled);
	}
	
	public void ShowGameOver()
	{
		
	}
}
