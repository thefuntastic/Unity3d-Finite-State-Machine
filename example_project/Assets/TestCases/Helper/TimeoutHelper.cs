using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeoutHelper : MonoBehaviour
{

	void Awake()
	{

	}

	//We call this on separate game object so that the active state of the caller has no bearing. 
	public void SetTimeout(Action callback, float duration)
	{
		StartCoroutine(DoTimeout(callback, duration));
	}

	public IEnumerator DoTimeout(Action callback, float duration)
	{
		Debug.Log("Starting timeout " + Time.time );
		yield return new WaitForSeconds(duration);

		Debug.Log("Ending timeout " + Time.time);
		if (callback != null) callback();
	}
}
