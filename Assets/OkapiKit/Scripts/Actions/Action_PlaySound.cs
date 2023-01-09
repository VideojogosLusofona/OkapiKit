using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Action_PlaySound : Action
{
    [SerializeField] 
    private AudioClip   clip;
    [SerializeField, MinMaxSlider(0.0f, 1.0f)]
    private Vector2     volume = Vector2.one;
    [SerializeField, MinMaxSlider(0.0f, 2.0f)]
    private Vector2     pitch = Vector2.one;

    public override string GetRawDescription(string ident)
    {
        if (clip == null) return "Plays a sound";

        return $"Plays sound {clip.name}";
    }
    public override void Execute()
    {
        if (!enableAction) return;

        SoundManager.PlaySound(clip, Random.Range(volume.x, volume.y), Random.Range(pitch.x, pitch.y));
    }
}
