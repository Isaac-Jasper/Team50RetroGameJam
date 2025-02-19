using UnityEngine;

[System.Serializable]
public class SoundData
{
    public float MusicVolume { get; set; }
    public float SoundEffectVolume { get; set;}
    public float MasterVolume { get; set;}
    public SoundData(GameSoundController data) {
        MusicVolume = data.MusicVolume;
        SoundEffectVolume = data.SoundEffectVolume;
        MasterVolume = data.MasterVolume;
    }

    public void UpdateSaveData(GameSoundController data) {
        MusicVolume = data.MusicVolume;
        SoundEffectVolume = data.SoundEffectVolume;
        MasterVolume = data.MasterVolume;
    }
}
