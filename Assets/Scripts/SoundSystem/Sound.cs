using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound //Creating a class for Sound objects which include:
{
    public string name; //Sound name

    public AudioClip clip; //The clip itself, .mp3, .wav, etc.

    [Range(0f, 1f)] //Creates a slider range between 0-1
    public float volume; //Volume of that audio clip

    [Range(.1f, 3f)] //Creates a slider range between .1-3
    public float pitch; //Pitch of that audio clip

    public bool loop; //select if the audio loops

    [HideInInspector] //Hides the next variable from the inspector tab
    public AudioSource source; //Creates an AudioSource object, unity's built-in audio files, for the object we create
}
