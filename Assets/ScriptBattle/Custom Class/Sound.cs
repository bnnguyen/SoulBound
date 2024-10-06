using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    [Range(0f, 10f)]
    public float startTime;

    public bool Loop = false;
    public bool PlayInAwake = false;

    [HideInInspector]
    public AudioSource source;
}
