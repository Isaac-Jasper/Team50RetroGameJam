using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundController : MonoBehaviour
{
    public static GameSoundController i { get; private set; }

    public enum Sound { //a reference variable of each sound which, when used with GameSoundController.SoundAudioClip, will relate to an audio clip
        //the catagories are spaced out for convenience as if they are not places that reference that variable will not have the same value is it is incrementing
        //eg. debug and developer sounds range from 0-99 in this catagory, and then UI sounds in 100-199, player in 200-299, ect.
        Debug_TestSound1 = 0, 
        Debug_TestSound2 = 1,
        Music_TestMusic1 = 10000,
        Music_TestMusic2 = 10001
    }
    [SerializeField]
    private float musicFadeSpeed = 50;

    [SerializeField]
    public SoundAudioClip[] soundAudioClipArray;
    [SerializeField]
    public SoundAudioClip[] MusicSource;

    [SerializeField]
    private AudioSource SFX;
    [SerializeField]
    private AudioSource CurrentMusic;
    private AudioSource NextMusic;

    public static Dictionary<Sound, SoundAudioClip> SoundDictonary; 
    public float MusicVolume
                , SoundEffectVolume
                , MasterVolume
                , HiddenVolumeController;
    public void PlaySound(Sound sound) {
        if ((int) sound >= 10000) {
            Debug.LogWarning("Use PlayMusic instead");
            PlayMusic(sound);
            return;
        }
        //use audioSource.[settings to set] 
        //in these spaces to change the values 
        //of played audio
        SFX.volume = getTotalSFXVolume();
        SFX.PlayOneShot(SoundDictonary[sound].audioClip);
    }

    public void PlayMusic(Sound music) {
        if ((int) music < 10000) {
            Debug.LogWarning("Use PlaySound instead");
            PlaySound(music);
            return;
        }
        //use audioSource.[settings to set] 
        //in these spaces to change the values 
        //of played audio

        NextMusic.loop = true;
        StopCoroutine("FadeMusicTransition");

        NextMusic.volume = 0;
        CurrentMusic.volume = getTotalMusicVolume();
        if (CurrentMusic.clip != null)
            StartCoroutine(FadeMusicTransition(music));
        else {
            CurrentMusic.loop = true;
            CurrentMusic.PlayOneShot(SoundDictonary[music].audioClip);
        }
    }

    public IEnumerator FadeMusicTransition(Sound music) {
        NextMusic.volume = 0;
        NextMusic.PlayOneShot(SoundDictonary[music].audioClip);
        while (NextMusic.volume <= getTotalMusicVolume() - 0.02) {
            NextMusic.volume += musicFadeSpeed / 100;
            CurrentMusic.volume -= musicFadeSpeed / 100;
            yield return null;
        }

        CurrentMusic.Stop();
        AudioSource temp = CurrentMusic;
        CurrentMusic = NextMusic;
        NextMusic = temp;
    }

    private void Awake() {
        if (i != null && i != this) {
            Destroy(this.gameObject);
        } else {
            i = this;
        }
        DontDestroyOnLoad(this.gameObject);
        Initialize();
    }

    private void Initialize() {
        SoundDictonary = new Dictionary<Sound, SoundAudioClip>();
        for (int i = 0; i < soundAudioClipArray.Length; i++) {
            SoundDictonary.Add(soundAudioClipArray[i].sound, soundAudioClipArray[i]);
        } 

        for (int i = 0; i < MusicSource.Length; i++) {
            SoundDictonary.Add(MusicSource[i].sound, MusicSource[i]);
        } 
    }

    private void Update() {
        for (int i = 0; i < MusicSource.Length; i++) {
            CurrentMusic.volume = getTotalMusicVolume();
        }
    }

    private float getTotalSFXVolume() {
        return MasterVolume * SoundEffectVolume * HiddenVolumeController;
    }

    private float getTotalMusicVolume() {
        return MasterVolume * SoundEffectVolume * HiddenVolumeController;
    }
}

[System.Serializable]
public class SoundAudioClip {
    public GameSoundController.Sound sound;
    public AudioClip audioClip;
}
