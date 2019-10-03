using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
	public bool LastExit = false;

    private void OnTriggerEnter(Collider other)
	{
		if(!LastExit)
		{
			LevelManager.Current.GoToNextLevel();
		}
		else
		{
			GameManager.Current.Victory();
		}
		
		gameObject.GetComponent<Animation>().Play();
	}
}
