using UnityEngine;
using System.Collections;

//All states should use the Serializable attribute if you want them to be visible in the inspector
[System.Serializable]
public class StateInit : FSMState<Main, Main.States>
{
	//By referencing the next state as a variable visible in the inspector, it makes it very easy to modify the state changing logic
	public Main.States nextState = Main.States.GamePlay;
	
	//Because propertie are now visible in the inspector, we can easily change state behaviours
	public float delay = 1;
	
	//FSM needs to function to keep track of the different states
	public override Main.States StateID {
		get {
			return Main.States.Init;
		}
	}
	
	public override void Enter ()
	{
		
		Debug.Log(string.Format("Game will start in {0} seconds", delay));
		
		//You can reference the owner class via "entity". In this case Main.
		entity.EnablePlayer(false);
		
		entity.ChangeState(nextState, delay);
	}

	public override void Execute ()
	{
		
	}

	public override void Exit ()
	{
		
	}

			
}