using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnitSoundController : MonoBehaviour
{
    public AudioEvent onHitSound;

    private AudioSource audioSource;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Hit()
    {
        onHitSound.Play(audioSource);
    }
}
