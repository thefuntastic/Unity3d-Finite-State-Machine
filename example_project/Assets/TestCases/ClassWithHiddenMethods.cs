using System;
using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class ClassWithHiddenMethods : MonoBehaviour 
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

	//Now that warning code has been removed this will no longer cause an error
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
		var fsm = StateMachine<States>.Initialize(this);
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
