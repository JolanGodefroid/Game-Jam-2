using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
	public GameObject MainMenuCanvas, OptionsMenuCanvas, CreditsMenuCanvas;
	public AudioMixer AudioMixer;
	public Slider MasterVolumeSlider;
	public Slider GammeSlider;
	public PostProcessProfile profile;

	public enum MenuState
	{
		InMainMenu,
		InOptionsMenu,
		InCreditsMenu,
	}
	public MenuState currentMenuState = MenuState.InMainMenu;

	private ColorGrading colorGrading;

	private void Start()
	{
		profile.TryGetSettings(out colorGrading);
		
		OptionsMenuCanvas.SetActive(false);
		CreditsMenuCanvas.SetActive(false);
		MainMenuCanvas.SetActive(true);

		MasterVolumeSlider.value = 0f;
		AudioMixer.SetFloat("Master", MasterVolumeSlider.value);

		GammeSlider.value = 0f;
		colorGrading.gamma.value = new Vector4(0f, 0f, 0f, GammeSlider.value);
	}

	public void SetMasterVolumeLevel(float sliderValue)
    {
        AudioMixer.SetFloat("Master", MasterVolumeSlider.value);
    }

	public void SetGammaLevel(float sliderValue)
	{
		colorGrading.gamma.value = new Vector4(0f, 0f, 0f, GammeSlider.value);
	}

	public void LoadMainMenuCanvas()
	{
		OptionsMenuCanvas.SetActive(false);
		CreditsMenuCanvas.SetActive(false);
		MainMenuCanvas.SetActive(true);
	}

	public void LoadOptionsMenu()
	{
		MainMenuCanvas.SetActive(false);
		OptionsMenuCanvas.SetActive(true);
	}

	public void LoadCreditsMenu()
	{
		MainMenuCanvas.SetActive(false);
		CreditsMenuCanvas.SetActive(true);
	}

	public void ChangeScene(int sceneNumber)
	{
		SceneManager.LoadScene(sceneNumber);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
