using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnitSoundController : MonoBehaviour
{
    public AudioEvent onHitSound;
    public AudioEvent footstepSound;

    private AudioSource audioSource;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Hit()
    {
        onHitSound.Play(audioSource);
    }

    public void FootL()
    {
        footstepSound.Play(audioSource);
    }

    public void FootR()
    {
        footstepSound.Play(audioSource);
    }
}
