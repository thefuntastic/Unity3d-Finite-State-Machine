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

	string oneTest;
	public void One_Enter()
	{
		//Spawns anon functions <One_Enter>m__80
		System.Action<string> anonFunc = (str) => { oneTest = str; };
	}

	//This should cause a warning;
	public void Twoo_Enter()
	{
		
	}

	//Double Underscors mimics unity hidden metadata functions, should be ignored
	public void Two__Ignore()
	{

	}

	public void Two_DoSomethingElse()
	{
		
	}

	public void TestInit () 
	{
		Initialize<States>();

		ChangeState(States.One);
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
