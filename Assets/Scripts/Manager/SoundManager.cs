using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource MusicSource, FxSource;
    public AudioClip[] FxClips;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    public void PlaySoundFX(int index)
    {
        FxSource.PlayOneShot(FxClips[index]);
    }
}
