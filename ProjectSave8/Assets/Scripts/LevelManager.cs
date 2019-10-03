using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LevelManager : MonoBehaviour
{
	public PostProcessProfile profile;
	public GameObject[] LevelsPrefabs = new GameObject[5];
	public Transform Player;

	private GameObject[] Levels = new GameObject[5];
	private Transform[] Checkpoints = new Transform[5];
	private int currentLevel = 0;
	private ColorGrading colorGrading;

	private static LevelManager current;
	public static LevelManager Current
    {
        get { return current; }
    }

	void Awake ()
    {
        current = this;
		profile.TryGetSettings(out colorGrading);
		colorGrading.colorFilter.value = Color.white;
	}

	private void Start()
	{
		for(int i = 0; i < LevelsPrefabs.Length; i++)
		{
			Levels[i] = Instantiate(LevelsPrefabs[i], new Vector3(0f, 0f, i * 100f), Quaternion.identity);
			Checkpoints[i] = Levels[i].transform.GetChild(0).transform;
		}

		Player.position = Checkpoints[currentLevel].position;
		Player.rotation = Checkpoints[currentLevel].rotation;

		EnableDisableLevels();
	}

	public void EnableDisableLevels()
	{
		for(int i = 0; i < LevelsPrefabs.Length; i++)
		{
			if(i == currentLevel)
			{
				Levels[i].SetActive(true);
			}
			else
			{
				Levels[i].SetActive(false);
			}
		}
	}

	public void ReloadLevel()
	{
		StartCoroutine(ReloadCurrentLevel());
	}

	IEnumerator ReloadCurrentLevel()
	{
		for(float i = 0f; i < 2f; i += Time.deltaTime)
		{
			colorGrading.colorFilter.value = Color.Lerp(Color.white, Color.black, i * 0.5f);
			yield return null;
		}

		Destroy(Levels[currentLevel]);
		Levels[currentLevel] = Instantiate(LevelsPrefabs[currentLevel], new Vector3(0f, 0f, currentLevel * 100f), Quaternion.identity);
		Checkpoints[currentLevel] = Levels[currentLevel].transform.GetChild(0).transform;

		Player.position = Checkpoints[currentLevel].position;
		Player.rotation = Checkpoints[currentLevel].rotation;
		PlayerControl.Current.ResetView();
		PlayerControl.Current.Alife = true;

		for(float i = 0f; i < 0.5f; i += Time.deltaTime)
		{
			colorGrading.colorFilter.value = Color.Lerp(Color.black, Color.white, i * 2f);
			yield return null;
		}
	}

	public void GoToNextLevel()
	{
		StartCoroutine(NextLevel());
	}

	IEnumerator NextLevel()
	{
		PlayerControl.Current.CanPlay = false;

		for(float i = 0f; i < 1f; i += Time.deltaTime)
		{
			colorGrading.colorFilter.value = Color.Lerp(Color.white, Color.black, i);
			yield return null;
		}

		currentLevel++;
		EnableDisableLevels();

		Player.position = Checkpoints[currentLevel].position;
		Player.rotation = Checkpoints[currentLevel].rotation;
		PlayerControl.Current.ResetView();

		for(float i = 0f; i < 0.5f; i += Time.deltaTime)
		{
			colorGrading.colorFilter.value = Color.Lerp(Color.black, Color.white, i * 2f);
			yield return null;
		}

		PlayerControl.Current.CanPlay = true;
	}
}
