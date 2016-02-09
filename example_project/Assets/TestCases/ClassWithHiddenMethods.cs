	using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ClassWithHiddenMethods : StateBehaviour 
{
	//Causes add_MyEvent & remove_MyEvent hidden method names
	public event Action MyEvent;

	public enum States
	{
		One,
		Two,
		Three,
	}

	void Awake()
	{
		TestInit();
	}

	public void One_Enter()
	{
		
	}

	//This should cause a warning;
	public void Twoo_Enter()
	{
		
	}

	//Double Underscors mimics unity hidden metadata functions, should be ignored
	public void Two__Ignore()
	{

	}

	public void TestInit () 
	{
		Initialize<States>();
	}

	//Causes get_TestProperty & set_TestProperty hidden method names
	public float TestProperty
	{
		get;
		set;
	}

	public void Broadcast ()
	{
		if (MyEvent != null) MyEvent();
	}
}
