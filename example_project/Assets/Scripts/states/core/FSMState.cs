using System;
public class FSMState <T, U>   {
	
	protected T entity;
	
	public void RegisterState(T entity)
	{
		this.entity = entity;
	}
	
	virtual public U StateID 
	{
		get{
			throw new ArgumentException("State ID not spicified in child class");
		}
	}

	virtual public void Enter (){}
		
	virtual public void Execute (){}

	virtual public void Exit(){}
}
