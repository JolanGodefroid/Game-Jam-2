using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRepulsor : MonoBehaviour
{
	public GameObject[] Objects = new GameObject[2];

	private Vector3 CenterPosition = Vector3.zero;
	private Vector3[] DirectionnalVectors = new Vector3[2];

	private enum Possibilities
	{
		Platform_Platform,
		Pillar_Platform,
		Platform_Pillar,
		Pillar_Pillar
	}
	private Possibilities currentPossibility;

    void Start()
    {
        if(Objects[0].layer == 10)
		{
			if(Objects[1].layer == 10)
			{
				currentPossibility = Possibilities.Platform_Platform;
			}
			else
			{
				currentPossibility = Possibilities.Platform_Pillar;
			}
		}

		if(Objects[0].layer == 11)
		{
			if(Objects[1].layer == 10)
			{
				currentPossibility = Possibilities.Pillar_Platform;
			}
			else
			{
				currentPossibility = Possibilities.Pillar_Pillar;
				Destroy(gameObject);
			}
		}

		switch(currentPossibility)
		{
			case Possibilities.Platform_Platform :

				CenterPosition = Objects[1].transform.position;

				break;
			
			case Possibilities.Platform_Pillar :

				CenterPosition = Objects[1].transform.position;

				break;
			
			case Possibilities.Pillar_Platform :

				CenterPosition = Objects[0].transform.position;

				break;
		}

		if(Objects[0].tag == Objects[1].tag)
		{
			switch(currentPossibility)
			{
				case Possibilities.Platform_Platform :

					DirectionnalVectors[0] = (Objects[0].transform.position - CenterPosition).normalized;

					break;
				
				case Possibilities.Platform_Pillar :

					DirectionnalVectors[0] = (Objects[0].transform.position - CenterPosition).normalized;

					break;
				
				case Possibilities.Pillar_Platform :

					DirectionnalVectors[1] = (Objects[1].transform.position - CenterPosition).normalized;

					break;
			}
		}
		else
		{
			switch(currentPossibility)
			{
				case Possibilities.Platform_Platform :

					DirectionnalVectors[0] = (CenterPosition - Objects[0].transform.position).normalized;

					break;
				
				case Possibilities.Platform_Pillar :

					DirectionnalVectors[0] = (CenterPosition - Objects[0].transform.position).normalized;

					break;
				
				case Possibilities.Pillar_Platform :

					DirectionnalVectors[1] = (CenterPosition - Objects[1].transform.position).normalized;

					break;
			}
		}

		switch(currentPossibility)
		{
			case Possibilities.Platform_Platform :

				MovePlatforms();

				break;

			case Possibilities.Pillar_Platform :

				MovePlatform();

				break;

			case Possibilities.Platform_Pillar :

				MovePlatform();

				break;
		}
    }

	private void MovePlatforms()
	{
		Vector3[] StartPositions = new Vector3[2];

		StartPositions[0] = Objects[0].transform.position;
		StartPositions[1] = Objects[1].transform.position;

		RaycastHit[] raycastHit= new RaycastHit[2];

		Physics.Raycast(Objects[0].transform.position, DirectionnalVectors[0], out raycastHit[0], 200f, PlayerControl.Current.layerMask);
		Physics.Raycast(Objects[1].transform.position, DirectionnalVectors[1], out raycastHit[1], 200f, PlayerControl.Current.layerMask);

		if(Objects[0].tag == Objects[1].tag)
		{
			StartCoroutine(DeplacePlatform(StartPositions[0], raycastHit[0].point, Objects[0]));
			StartCoroutine(DeplacePlatform(StartPositions[1], raycastHit[1].point, Objects[1]));
		}
		else
		{
			Vector3[] EndPositions = new Vector3[2];

			EndPositions[0] = (raycastHit[0].point + StartPositions[0]) * 0.5f;
			EndPositions[1] = (raycastHit[1].point + StartPositions[1]) * 0.5f;
			
			StartCoroutine(DeplacePlatform(StartPositions[0], EndPositions[0], Objects[0]));
			StartCoroutine(DeplacePlatform(StartPositions[1], EndPositions[1], Objects[1]));
		}
	}

	private void MovePlatform()
	{
		RaycastHit raycastHit;
		Vector3 StartPosition;

		switch(currentPossibility)
		{
			case Possibilities.Platform_Pillar :
				
				StartPosition = Objects[0].transform.position;

				Physics.Raycast(StartPosition, DirectionnalVectors[0], out raycastHit, 200f, PlayerControl.Current.layerMask);

				StartCoroutine(DeplacePlatform(StartPosition, raycastHit.point, Objects[0]));

				break;

			case Possibilities.Pillar_Platform :

				StartPosition = Objects[1].transform.position;

				Physics.Raycast(StartPosition, DirectionnalVectors[1], out raycastHit, 200f, PlayerControl.Current.layerMask);

				StartCoroutine(DeplacePlatform(StartPosition, raycastHit.point, Objects[1]));

				break;
		}
	}

	IEnumerator DeplacePlatform(Vector3 StartPosition, Vector3 EndPosition, GameObject Platform)
	{
		while(Vector3.Distance(Platform.transform.position, EndPosition) > 1f && Vector3.Distance(StartPosition, Platform.transform.position) < PlayerControl.Current.MaxPlatformTravelLength)
		{
			Platform.transform.position += (EndPosition - StartPosition).normalized * PlayerControl.Current.PlatformSpeed * Time.deltaTime;
			yield return null;
		}

		Destroy(gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(Objects[0].transform.position, DirectionnalVectors[0]);
		Gizmos.DrawRay(Objects[1].transform.position, DirectionnalVectors[1]);
	}
}
