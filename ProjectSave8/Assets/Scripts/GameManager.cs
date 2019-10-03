using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
	public GameObject MenuCanvas;
	public GameObject OptionsCanvas;
	public GameObject VictoryCanvas;
	public GameObject HelpCanvas;
	public GameObject CentralCursor;
	public AudioMixer audioMixer;
	public Slider GammeSlider;
	public Slider MasterVolumeSlider;
	public PostProcessProfile profile;

	private static GameManager current;
	public static GameManager Current
    {
        get { return current; }
    }

	public enum GameState
	{
		InMenu,
		InGame,
		End
	}
	public GameState currentGameState = GameState.InGame;

	private ColorGrading colorGrading;

	void Awake ()
    {
        current = this;

		CentralCursor.SetActive(true);
		VictoryCanvas.SetActive(false);
		MenuCanvas.SetActive(false);
		HelpCanvas.SetActive(false);

		Time.timeScale = 1f;
		
		profile.TryGetSettings(out colorGrading);
		GammeSlider.value = colorGrading.gamma.value.w;
		float temp;
		audioMixer.GetFloat("Master", out temp);
		MasterVolumeSlider.value = temp;
		
		OptionsCanvas.SetActive(false);
		MenuCanvas.SetActive(false);
	}

	private void Update()
	{
		switch(currentGameState)
		{
			case GameState.InGame :

				if(Input.GetKeyDown(KeyCode.Escape))
				{
					MenuCanvas.SetActive(true);

					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;

					Time.timeScale = 0f;
					currentGameState = GameState.InMenu;
				}

				if(Input.GetKeyDown(KeyCode.Tab))
				{
					HelpCanvas.SetActive(true);
					Time.timeScale = 0f;
				}
				if(Input.GetKeyUp(KeyCode.Tab))
				{
					HelpCanvas.SetActive(false);
					Time.timeScale = 1f;
				}

				break;

			case GameState.InMenu :

				if(Input.GetKeyDown(KeyCode.Escape))
				{
					MenuCanvas.SetActive(false);
					OptionsCanvas.SetActive(false);
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					Time.timeScale = 1f;
					currentGameState = GameState.InGame;
				}

				break;

			case GameState.End :

				if(Input.GetKeyDown(KeyCode.Escape))
				{
					BackToMainMenu();
				}

				if(Input.GetKeyDown(KeyCode.R))
				{
					Scene currentScene = SceneManager.GetActiveScene();
					SceneManager.LoadScene(currentScene.name);
				}

				break;
		}
	}

	public void Victory()
	{
		VictoryCanvas.SetActive(true);
		currentGameState = GameState.End;
		Time.timeScale = 0f;
	}

	public void SetMasterVolumeLevel(float sliderValue)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(MasterVolumeSlider.value) * 20);
    }

	public void SetGammaLevel(float sliderValue)
	{
		colorGrading.gamma.value = new Vector4(0f, 0f, 0f, GammeSlider.value);
	}

	public void LoadMainMenuCanvas()
	{
		OptionsCanvas.SetActive(false);
		MenuCanvas.SetActive(true);
	}

	public void LoadOptionsMenu()
	{
		MenuCanvas.SetActive(false);
		OptionsCanvas.SetActive(true);
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void Resume()
	{
		MenuCanvas.SetActive(false);
		Time.timeScale = 1f;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		currentGameState = GameState.InGame;
	}

	public void ReloadLevel()
	{
		Time.timeScale = 1f;
		MenuCanvas.SetActive(false);
		OptionsCanvas.SetActive(false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		currentGameState = GameState.InGame;
	}

	public void BackButton()
	{
		MenuCanvas.SetActive(true);
		OptionsCanvas.SetActive(false);
	}
}
