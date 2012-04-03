using UnityEngine;
using System.Collections.Generic;
using System;

public class FiniteStateMachine <T,U>  {
	private T Owner;
	private FSMState<T,U> CurrentState;
	private FSMState<T,U> PreviousState;
	private FSMState<T,U> GlobalState;
	
	private Dictionary<U,FSMState<T,U>> stateRef;
	
	public void Awake()
	{		
		CurrentState = null;
		PreviousState = null;
		GlobalState = null;
	}
	
	public FiniteStateMachine(T owner) {
		Owner = owner;
		stateRef = new Dictionary<U, FSMState<T, U>>();
	}

	public void  Update()
	{
		if (GlobalState != null)  GlobalState.Execute();
		if (CurrentState != null) CurrentState.Execute();
	}
 
	public void  ChangeState(FSMState<T,U> NewState)
	{	
		PreviousState = CurrentState;
 
		if (CurrentState != null)
			CurrentState.Exit();
 
		CurrentState = NewState;
 
		if (CurrentState != null)
			CurrentState.Enter();
	}
 
	public void  RevertToPreviousState()
	{
		if (PreviousState != null)
			ChangeState(PreviousState);
	}
		
	//Changing state via enum
	public void ChangeState(U stateID)
	{
		try
        {
            FSMState<T, U> state = stateRef[stateID];
			ChangeState(state);

        }
        catch (KeyNotFoundException)
        {
          	throw new Exception("There is no State assiciated with that definition");
        }

	}
	
	public FSMState<T, U> RegisterState(FSMState<T,U> state)
	{
		state.RegisterState(Owner);
		stateRef.Add(state.StateID, state);
		return state;
	}
	
	public void UnregisterState(FSMState<T,U> state)
	{
		stateRef.Remove(state.StateID);
		
	}
};