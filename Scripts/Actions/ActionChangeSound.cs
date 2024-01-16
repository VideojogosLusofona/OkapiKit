using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Sound")]
    public class ActionChangeSound : Action
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip   clip;
        [SerializeField, MinMaxSlider(0.0f, 1.0f)]
        private Vector2     volume = Vector2.one;
        [SerializeField, MinMaxSlider(0.0f, 2.0f)]
        private Vector2     pitch = Vector2.one;

        public override string GetActionTitle() => "Change Sound";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);
            string clipString = "to an undefined sound";

            if (audioSource == null)
            {
                AudioSource src = GetComponent<AudioSource>();
                if (src != null)
                {
                    desc += $"changes sound playing in this audio source " + clipString;
                }
                else
                {
                    desc += $"changes sound playing on an undefined audio source " + clipString;
                }
            }
            else
            {
                desc += $"changes sound playing on an audio source {audioSource.name} " + clipString;
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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (clip == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined sound to play!", "We need to know what sound to play"));
            }
            if (audioSource == null)
            {
                AudioSource src = GetComponent<AudioSource>();
                if (src != null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Target audio source not defined by reference!", "We should define everything explicitly, so the system doesn't have to guess what we want"));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target audio source not defined!", "To change a sound we need to know what audio source to change"));
                }
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            audioSource.clip = clip;
            audioSource.volume = Random.Range(volume.x, volume.y);
            audioSource.pitch = Random.Range(pitch.x, pitch.y);
            audioSource.Play();
        }
    }
}
