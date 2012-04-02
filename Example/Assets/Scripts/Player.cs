using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
	public event EventHandler<EventArgs> InstaWin;
	
	public float health;
	public float damageSpeed;

	private float startHealth;
	
	// Use this for initialization
	void Start ()
	{
		startHealth = health;
	}
	
	// Update is called once per frame
	void Update ()
	{
		health -= Time.deltaTime * damageSpeed;
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect (100,100,100,30), "Make Me Win"))
		{
			if (InstaWin != null)
			{
				InstaWin(this, null);
			}
		}
	}
	
	public void ResetHealth()
	{
		health = startHealth;
	}
}

