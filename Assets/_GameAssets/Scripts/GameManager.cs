using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    [SerializeField] AudioSource myAudioSource;

    private void Awake()
    {
        if(singleton == null)
            singleton = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void PlayAudio(AudioClip clip)
    {
        singleton.myAudioSource.PlayOneShot(clip);
    }
}
