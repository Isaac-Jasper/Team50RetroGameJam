using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; //array of all sounds that will then be called upon for whether you want to play that sound
    // i.e s[0].play();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds) //for-each loop to iterate through every item in sounds array
        {
            s.source = gameObject.AddComponent<AudioSource>(); //Create the Audio Source component to each object
            s.source.clip = s.clip; //Take the clip we inputted into the Audio Source

            s.source.volume = s.volume; //Take the volume setting we inputted into the Audio Source
            s.source.pitch = s.pitch; //Take the pitch setting we inputted into the Audio Source
            s.source.loop = s.loop;
        }
    }

    public void Start()
    {
        
    }

    public void Play(string name) //plays the sound
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); //What comes after the , is a Lambda Expression
        //These are quick functions for smaller tasks, it basically states a function with parameters (__i.e sound___) => (and runs) (___function task i.e sound.name == name__);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " cannot be found! Did you type the right name?");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name) //stops the sound from playing
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
}
