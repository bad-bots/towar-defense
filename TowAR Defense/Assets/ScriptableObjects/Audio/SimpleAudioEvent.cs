using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip[] clips;

    [MinMaxRange(0, 2)]
    public RangedFloat volume = new RangedFloat(0.9f, 1.1f);
    [MinMaxRange(0, 2)]
    public RangedFloat pitch = new RangedFloat(0.9f, 1.1f);

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.Play();
    }
}
