using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip[] soundEffects;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        musicSource.volume = 0.2f;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ResumeMusic()
    {
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySound(int index)
    {
        if (index < 0 || index >= soundEffects.Length)
        {
            Debug.LogWarning("Sound index out of range!");
            return;
        }
        sfxSource.PlayOneShot(soundEffects[index]);
    }
}
