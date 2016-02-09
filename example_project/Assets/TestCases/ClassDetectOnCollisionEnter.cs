using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;

public class ClassDetectOnCollisionEnter : StateBehaviour
{
	public enum States
	{
		One,
		Two,
	}

	public bool oneHasCollision = false;
	public bool twoHasCollision = false;


	void Awake()
	{
		Initialize<States>();
		ChangeState(States.One);
	}


	void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.collider);
	}

	void One_OnCollisionEnter(Collision collision)
	{
		oneHasCollision = true;
		Debug.Log("one has on colliision " + collision.collider);
	}
}
