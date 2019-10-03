using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
	{
		PlayerControl.Current.Alife = false;

		LevelManager.Current.ReloadLevel();
	}
}
