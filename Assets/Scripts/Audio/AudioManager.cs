using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public Sound[] sounds;
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.pitch = s.pitch;
        }
        Play(Constants.BGM_Theme);
       
    }

    public void Play(string name, float pitch)
    {
        if (name == "Null")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sounds: " + name + " is not found!");
        }
        s.source.pitch = pitch;
        s.source.Play();
    }
    public void Play(string name)
    {
        if (name == "Null")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sounds: " + name + " is not found!");
        }
        // s.source.pitch = pitch;
        s.source.Play();
    }
    public void Stop(string name)
    {
        if (name == "Null")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sounds: " + name + " is not found!");
        }
        // s.source.pitch = pitch;
        s.source.Stop();
    }
	public void PlayOnce(string name)
	{
		if (name == "Null")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sounds: " + name + " is not found!");
        }
        // s.source.pitch = pitch;
		if (!s.source.isPlaying)
		{
			s.source.Play();
		}
		
	}
}
