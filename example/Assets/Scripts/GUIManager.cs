using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	public delegate void Callback();
	
	private bool showHealth = false;
	private float health = 0;
	private bool showGameOver = false;
	private Callback gameOverCallback;
	
	
	void OnGUI()
	{
		if(showHealth)
		{
			GUI.Label(new Rect( 100, 50, 135, 30), "Health: " + health.ToString("N2"));
		}
		
		if(showGameOver)	
		{
			if(GUI.Button(new Rect (100,150,100,30), "Try Again"))
			{
				if (gameOverCallback != null)
				{
					gameOverCallback();
				}
				showGameOver = false;
			}
		}
		
	}
	
	public bool ShowHealth
	{
		set{showHealth = value;}
	}
	
	public void UpdateHealth(float health)
	{
		this.health = health;
	}
	
	public void ShowGameOver(bool visible, Callback cb = null)
	{
		showGameOver = visible;
		gameOverCallback = cb;
	}
	
}
