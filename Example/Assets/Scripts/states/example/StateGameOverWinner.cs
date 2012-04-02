using UnityEngine;
using System.Collections;

//All states should use the Serializable attribute if you want them to be visible in the inspector
[System.Serializable]
public class StateGameOverWinner : FSMState<Main, Main.States>
{
	public Main.States replayState = Main.States.GamePlay;
	
	public override Main.States StateID {
		get {
			return Main.States.GameOverWinner;
		}
	}

	public override void Enter ()
	{
		Debug.Log("VICTORY!");
		
		entity.gui.ShowGameOver(true, new GUIManager.Callback(HandleTryAgain));
	}

	public override void Execute ()
	{
		
	}

	public override void Exit ()
	{
		
	}
	
	private void HandleTryAgain()
	{
		entity.ChangeState(replayState);
	}
}

