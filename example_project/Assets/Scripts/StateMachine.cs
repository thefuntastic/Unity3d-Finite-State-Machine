using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
	public event Action<object> Changed;

	private Dictionary<object, StateMapping> mappings;
	private object currentState;
	private object lastState = null;

	private object nextState = null;
	private bool queuedChange;
	private float changeStartTime;
	private float changeDelay;

	public StateMachine()
	{
		mappings = new Dictionary<object, StateMapping>();
	}


	public void AddState(StateMapping mapping)
	{
		mappings.Add(mapping.state, mapping);
	}

	public void AddState(object stateKey, Action stateEnter, Action stateExit, Action stateUpdate)
	{
		AddState(new StateMapping(stateKey, stateEnter, stateExit, stateUpdate));
	}

	public void UpdateStates()
	{
		if (queuedChange && Time.time - changeStartTime > changeDelay)
		{
			CompleteStateChange(nextState);
		}

		if (currentState != null)
		{
			if (mappings[currentState].stateUpdate != null)
			{
				mappings[currentState].stateUpdate();
			}
		}
	}

	public void ChangeState(object newState)
	{
		ChangeState(newState, -1);
	}

	public void ChangeState(object newState, float delay)
	{
		queuedChange = false;

		if (currentState != null)
		{
			if (mappings[currentState].stateExit != null) mappings[currentState].stateExit();
		}

		if (!mappings.ContainsKey(newState))
		{
			throw new ArgumentException("No mapping for " + newState + " is defined");
		}


		if (delay <= 0)
		{
			CompleteStateChange(newState);
		}
		else
		{
			queuedChange = true;
			nextState = newState;
			changeDelay = delay;
			changeStartTime = Time.time;
		}

	}

	private void CompleteStateChange(object newState)
	{
		queuedChange = false;
		nextState = null;
		lastState = currentState;
		currentState = newState;

		if(Changed != null) Changed(newState);

		if (mappings[newState].stateEnter != null) mappings[newState].stateEnter();
	}

	public void ChangeToLastState()
	{
		ChangeState(lastState);
	}

	public object CurrentState
	{
		get { return currentState; }
	}

	public bool IsEqualCurrentState(object newState)
	{
		if (currentState == null)
		{
			return newState == null;
		}

		return currentState.Equals(newState);
	}

	public bool IsEqualLastState(object newState)
	{
		if (lastState == null)
		{
			return newState == null;
		}

		return lastState.Equals(newState);
	}

	public object LastState
	{
		get { return lastState; }
	}
}

public class StateMapping
{
	public readonly object state;
	public Action stateEnter;
	public Action stateExit;
	public Action stateUpdate;

	public StateMapping(object state, Action stateEnter, Action stateExit, Action stateUpdate)
	{
		this.state = state;
		this.stateEnter = stateEnter;
		this.stateExit = stateExit;
		this.stateUpdate = stateUpdate;

	}
}
