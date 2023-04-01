using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionPlaySound : Action
    {
        [SerializeField]
        private AudioClip clip;
        [SerializeField, MinMaxSlider(0.0f, 1.0f)]
        private Vector2 volume = Vector2.one;
        [SerializeField, MinMaxSlider(0.0f, 2.0f)]
        private Vector2 pitch = Vector2.one;

        public override string GetActionTitle() => "Play Sound";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);
            if (clip == null)
            {
                desc += "plays an undefined sound";
            }
            else
            {
                desc += $"plays sound {clip.name}";
            }

            if (volume.x == volume.y)
            {
                desc += $" at volume {volume.x}";
            }
            else
            {
                desc += $" with a volume in the range [{volume.x},{volume.y}]";
            }

            if (pitch.x == pitch.y)
            {
                desc += $" and at pitch {pitch.x}";
            }
            else
            {
                desc += $" and a pitch in the range [{pitch.x},{pitch.y}]";
            }

            return desc;
        }
        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            SoundManager.PlaySound(clip, Random.Range(volume.x, volume.y), Random.Range(pitch.x, pitch.y));
        }
    }
}