using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	static private PlayerControl current;
	static public PlayerControl Current
    {
        get { return current; }
    }

	[Header("Objects")]
	public Transform CameraTransform;
	public Transform PositiveHotPoint;
	public Transform NegativeHotPoint;
	public GameObject[] PowerIcons = new GameObject[2];
	public GameObject PlateformRepulsor;
	public Animator animator;
	public GameObject audioSourcePrefab;
	public AudioClip[] sounds;

	[Header("Control")]
    [Range(0f, 300f)] public float MouseSensitivity = 150f;
	[Range(1f, 5f)] public float MovementSpeed = 4f;
	[Range(5f, 10f)] public float SprintMoveSpeed = 7f;
	[Range(1f, 30f)] public float JumpIntensity = 10f;

	[Header("Powers")]
	[Range(0f, 100f)] public float InteractionRange = 50f;
	[Range(0f, 5f)] public float MinimalAttractionDistance = 1.5f;
	[Range(0.1f, 10f)] public float PlatformSpeed = 4f;
	[Range(0f, 10f)] public float AttractionRepulsionTime = 4f;
	[Range(0f, 100f)] public float MaxPlatformTravelLength = 30f;

	[Header("RayCast")]
	public float GroundCheckRayLenght;
	public LayerMask layerMask;
	public LayerMask GroundLayerMask;
	public LayerMask PlatformLayerMask;
	public LayerMask PlateformPillarLayerMask;

	private Rigidbody rb;
	private Plateform PlateformScript;
	private Transform PlateformTransform;
	private GameObject[] SelectedObject = new GameObject[2];
	private float MouseX;
	private float MouseY;
	private float yAxisClamp = 0f;
	private float currentMoveSpeed;
	private bool RightClic = false;
	private bool LeftClic = false;
	private bool IsFlying = false;
	private bool IsMoving = false;
	private bool CanMove = true;
	[HideInInspector] public bool Alife = true;
	[HideInInspector] public bool CanPlay = true;

	private enum PowerState
	{
		Player,
		Platforms
	}
	private PowerState currentPowerState = PowerState.Player;

	[HideInInspector] public GameObject TargetedPlatform = null;

	private void Awake()
	{
		current = this;
		rb = GetComponent<Rigidbody>();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		currentMoveSpeed = MovementSpeed;
	}

	private void Update()
	{
		if(!Alife || !CanPlay)
		{
			return;
		}	

		#region Selection & Outline System

		if(!(TargetedPlatform != null && (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))))
		{
			if(currentPowerState == PowerState.Player)
			{
				RaycastHit hit;
				if(Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hit, InteractionRange, PlatformLayerMask))
				{
					GameObject touchedObject = hit.collider.gameObject;

					if(TargetedPlatform == null && touchedObject.layer == 10)
					{
						TargetedPlatform = touchedObject;
					}
					else
					{
						if(touchedObject != TargetedPlatform)
						{
							TargetedPlatform = touchedObject;
						}
					}
				}
				else
				{
					if(TargetedPlatform != null)
					{
						TargetedPlatform = null;
					}
				}
			}
			else
			{
				RaycastHit hit;
				if(Physics.Raycast(CameraTransform.position, CameraTransform.forward, out hit, InteractionRange, PlateformPillarLayerMask))
				{
					GameObject touchedObject = hit.collider.gameObject;

					if(TargetedPlatform == null)
					{
						TargetedPlatform = touchedObject;
					}
					else
					{
						if(touchedObject != TargetedPlatform)
						{
							TargetedPlatform = touchedObject;
						}
					}
				}
				else
				{
					if(TargetedPlatform != null)
					{
						TargetedPlatform = null;
					}
				}
			}
		}

		#endregion

		#region PlayerControl

		if(Input.GetKey(KeyCode.Mouse0))
		{
			LeftClic = true;
			CanMove = false;
			SetAnimator(false, false, false, false, true);
		}
		else
		{
			LeftClic = false;
			CanMove = true;

			if(Input.GetKey(KeyCode.Mouse1) && !LeftClic)
			{
				SetAnimator(false, false, false, true, false);
				RightClic = true;
				CanMove = false;
			}
			else
			{
				RightClic = false;
				CanMove = true;

				if(Input.GetKeyDown(KeyCode.LeftShift))
				{
					SetAnimator(false, false, true, false, false);
				}
				else if(Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") != 0f)
				{
					SetAnimator(false, true, false, false, false);
				}
				else
				{
					SetAnimator(true, false, false, false, false);
				}
			}
		}
		

		if(Input.GetKeyDown(KeyCode.A))
		{
			if(currentPowerState == PowerState.Player)
			{
				currentPowerState = PowerState.Platforms;
				PowerIcons[0].SetActive(false);
				PowerIcons[1].SetActive(true);
			}
			else
			{
				currentPowerState = PowerState.Player;
				PowerIcons[0].SetActive(true);
				PowerIcons[1].SetActive(false);
			}
		}

		if(Input.GetKeyDown(KeyCode.E))
		{
			ResetSelectedObject();
		}

		if(currentPowerState == PowerState.Platforms && Input.GetKeyDown(KeyCode.Mouse0) && TargetedPlatform != null)
		{
			if(SelectedObject[0] == null)
			{
				SelectedObject[0] = TargetedPlatform;
				SelectedObject[0].GetComponent<Plateform>().Selected = true;
			}
			else if(SelectedObject[0] != TargetedPlatform)
			{
				SelectedObject[1] = TargetedPlatform;
				SelectedObject[1].GetComponent<Plateform>().Selected = true;
				SecondPower();
			}
		}

		#endregion

		#region Mouvement Gestion

		transform.Rotate(Vector3.up, MouseX);

		if(CanMove)
		{
			if(IsFlying)
			{
				if(Physics.Raycast(transform.position, Vector3.down, GroundCheckRayLenght, GroundLayerMask))
				{
					IsFlying = false;
					
					if(Input.GetKey(KeyCode.LeftShift))
					{
						currentMoveSpeed = SprintMoveSpeed;
					}
					else
					{
						currentMoveSpeed = MovementSpeed;
					}
				}
			}

			if(Input.GetKeyDown(KeyCode.Space) && !IsFlying)
			{
				StartCoroutine(Jump());
			}

			if(Input.GetKeyDown(KeyCode.LeftShift) && !IsFlying)
			{
				StopCoroutine(BackToNormalSpeed());
				currentMoveSpeed = SprintMoveSpeed;
			}

			if(Input.GetKeyUp(KeyCode.LeftShift))
			{
				StartCoroutine(BackToNormalSpeed());
			}
		}

		#endregion

		#region CameraRotation

		MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
		MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

		yAxisClamp += MouseY;

		if(yAxisClamp > 90f)
		{
			yAxisClamp = 90f;
			MouseY = 0f;
		}
		if(yAxisClamp < -90f)
		{
			yAxisClamp = -90f;
			MouseY = 0f;
		}
		
		CameraTransform.Rotate(Vector3.left * MouseY);

		#endregion
	}

	private void FixedUpdate()
	{
		if(!Alife)
		{
			return;
		}

		Vector3 Deplacement = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")) * currentMoveSpeed * Time.deltaTime;
		rb.position += Deplacement;

		if(Deplacement == Vector3.zero && rb.velocity == Vector3.zero)
		{
			IsMoving = false;
		}
		else
		{
			IsMoving = true;
		}
	
		#region Power
		
		if(TargetedPlatform != null && TargetedPlatform.layer == 10 && !IsMoving)
		{
			RaycastHit hit;

			if(currentPowerState == PowerState.Player && (!Physics.Raycast(transform.position, Vector3.down, out hit, GroundCheckRayLenght, PlatformLayerMask) || (Physics.Raycast(transform.position, Vector3.down, out hit, GroundCheckRayLenght, PlatformLayerMask) && hit.collider.gameObject != TargetedPlatform)))
			{
				if(LeftClic) //RightHand - Positive
				{
					PlateformScript = TargetedPlatform.GetComponent<Plateform>();
					PlateformTransform = TargetedPlatform.transform;
					
					RaycastHit raycastHit;
					
					switch(PlateformScript.currentCharge)
					{
						case Plateform.Charge.Negative :

							if(Vector3.Distance(transform.position, TargetedPlatform.transform.position) > MinimalAttractionDistance)
							{
								Physics.Raycast(PlateformTransform.position, (transform.position - PlateformTransform.position).normalized, out raycastHit, 100f, layerMask);
								StartCoroutine(MovePlatform(PlateformTransform.position, raycastHit.point - (transform.position - PlateformTransform.position).normalized, TargetedPlatform, "LeftClic"));
							}

							break;

						case Plateform.Charge.Positive :

							Physics.Raycast(PlateformTransform.position, (PlateformTransform.position - transform.position).normalized, out raycastHit, 100f, layerMask);
							StartCoroutine(MovePlatform(PlateformTransform.position, raycastHit.point - (PlateformTransform.position - transform.position).normalized, TargetedPlatform, "LeftClic"));

							break;
					}
				}
				if(RightClic) //LeftHand - Negative
				{
					PlateformScript = TargetedPlatform.GetComponent<Plateform>();
					PlateformTransform = TargetedPlatform.transform;
					
					RaycastHit raycastHit;

					switch(PlateformScript.currentCharge)
					{
						case Plateform.Charge.Negative :

							Physics.Raycast(PlateformTransform.position, (PlateformTransform.position - transform.position).normalized, out raycastHit, 100f, layerMask);
							StartCoroutine(MovePlatform(PlateformTransform.position, raycastHit.point - (PlateformTransform.position - transform.position).normalized, TargetedPlatform, "RightClic"));

							break;

						case Plateform.Charge.Positive :

							if(Vector3.Distance(transform.position, TargetedPlatform.transform.position) > MinimalAttractionDistance)
							{
								Physics.Raycast(PlateformTransform.position, (transform.position - PlateformTransform.position).normalized, out raycastHit, 100f, layerMask);
								StartCoroutine(MovePlatform(PlateformTransform.position, raycastHit.point - (transform.position - PlateformTransform.position).normalized, TargetedPlatform, "RightClic"));
							}

							break;
					}
				}
			}
		}

		#endregion
	}
	IEnumerator MovePlatform(Vector3 StartPosition, Vector3 EndPosition, GameObject Platform, string Clic)
	{
		if(Clic == "RightClic")
		{
			while(Vector3.Distance(Platform.transform.position, EndPosition) > 1f && Input.GetKey(KeyCode.Mouse1))
			{
				Platform.transform.position += (EndPosition - StartPosition).normalized * PlatformSpeed * Time.deltaTime;

				yield return null;
			}
		}
		if(Clic == "LeftClic")
		{
			while(Vector3.Distance(Platform.transform.position, EndPosition) > 1f && Input.GetKey(KeyCode.Mouse0))
			{
				Platform.transform.position += (EndPosition - StartPosition).normalized * PlatformSpeed * Time.deltaTime;

				yield return null;
			}
		}
	}

	private void SetAnimator(bool CanIdle, bool CanWalk, bool CanRun, bool RightClic, bool LeftClic)
	{
		animator.SetBool("CanIdle", CanIdle);
		//animator.SetBool("CanWalk", LeftClic);
		//animator.SetBool("CanRun", LeftClic);
		animator.SetBool("RightClic", RightClic);
		animator.SetBool("LeftClic", LeftClic);
	}


	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Platform"))
		{
			transform.SetParent(other.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Platform"))
		{
			transform.parent = null;
		}
	}

	private void SecondPower()
	{
		GameObject PlateformRepulsorInstance = Instantiate(PlateformRepulsor, Vector3.zero, Quaternion.identity);
		PlatformRepulsor platformRepulsorScript = PlateformRepulsorInstance.GetComponent<PlatformRepulsor>();
		platformRepulsorScript.Objects[0] = SelectedObject[0];
		platformRepulsorScript.Objects[1] = SelectedObject[1];
		ResetSelectedObject();
	}

	private void ResetSelectedObject()
	{
		if(SelectedObject[0] != null)
		{
			SelectedObject[0].GetComponent<Plateform>().Selected = false;
			SelectedObject[0] = null;
		}
		if(SelectedObject[1] != null)
		{
			SelectedObject[1].GetComponent<Plateform>().Selected = false;
			SelectedObject[1] = null;
		}
	}

	public void ResetView()
	{
		//MouseX = 0f;
		//MouseY = 0f;
		CameraTransform.Rotate(Vector3.zero);
		yAxisClamp = 0f;
	}

	IEnumerator Jump()
	{
		rb.velocity = new Vector3(0f, JumpIntensity, 0f);

		yield return new WaitForSeconds(0.1f);

		IsFlying = true;
	}

	IEnumerator BackToNormalSpeed()
	{
		for(float i = 0f; i < 0.5f; i += Time.deltaTime)
		{
			currentMoveSpeed = Mathf.Lerp(SprintMoveSpeed, MovementSpeed, i * 2f);

			yield return null;
		}
	}

	private void PlaySound(int SoundNum, bool UseRandomPitch)
	{
		GameObject AudioPrefabInstance = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
		AudioSource audioSource = AudioPrefabInstance.GetComponent<AudioSource>();
		audioSource.clip = sounds[SoundNum];

		if(UseRandomPitch)
		{
			audioSource.pitch = Random.Range(0.9f, 1.1f);
		}
		else
		{
			audioSource.pitch = 1f;
		}

		audioSource.Play();

		Destroy(AudioPrefabInstance, 5f);
	}
}
