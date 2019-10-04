using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public AudioClip[] musics;
	private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

	IEnumerator PlayMusics()
	{
		while(true)
		{
			int musicToPlay = Random.Range(0, musics.Length);
			audioSource.clip = musics[musicToPlay];
			audioSource.PlayOneShot(audioSource.clip);
			yield return new WaitForSeconds(audioSource.clip.length);
		}
	}
}
