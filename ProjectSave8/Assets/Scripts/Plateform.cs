using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateform : MonoBehaviour
{
	public bool Selected = false;
	private bool plateform = false;

	private Material material;

	[HideInInspector] public enum Charge
	{
		Positive,
		Negative
	}
	[HideInInspector] public Charge currentCharge;

	private void Awake()
	{
		if(gameObject.layer == 10)
		{
			plateform = true;
		}
		else
		{
			plateform = false;
		}

		if(plateform)
		{
			material= GetComponent<MeshRenderer>().material;
		}
		else
		{
			material = transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().material;
		}
	}

	private void Start()
	{
		if(gameObject.CompareTag("NegativeCharge"))
		{
			currentCharge = Charge.Negative;
		}
		else if(gameObject.CompareTag("PositiveCharge"))
		{
			currentCharge = Charge.Positive;
		}
	}

	private void Update()
	{
		if(PlayerControl.Current.TargetedPlatform == gameObject || Selected)
		{
			if(plateform)
			{
				material.SetFloat("_OutlineWidth", 0.04f);
			}
			else
			{
				for(int i = 0; i < 4; i++)
			{
				material.SetFloat("_OutlineWidth", 0.05f);
			}
			}
		}
		else
		{
			if(plateform)
			{
				material.SetFloat("_OutlineWidth", 0f);
			}
			else
			{
				material.SetFloat("_OutlineWidth", 0f);
			}
		}
	}
}
