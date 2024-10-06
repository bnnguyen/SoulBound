using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.Loop;
            s.source.playOnAwake = s.PlayInAwake;

            s.source.time = s.startTime;
        }
    }

    public void Start()
    {
        StartCoroutine(StartSoundtrack());
    }

    IEnumerator StartSoundtrack()
    { 
        Play("Theme");
        yield return new WaitForSeconds(14);
        Play("BattleLoop");
    }

    public void Play(string Name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == Name);
        s.source.Play();
    }

    public void Stop(string Name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == Name);
        s.source.Stop();
    }
}
