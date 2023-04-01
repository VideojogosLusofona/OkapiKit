using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace OkapiKit
{
    public class SoundManager : OkapiElement
    {
        public static SoundManager instance;

        [SerializeField] private AudioMixerGroup defaultMixerOutput;


        List<AudioSource> audioSources = new List<AudioSource>();

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void _PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
        {
            var audioSource = GetSource();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.outputAudioMixerGroup = defaultMixerOutput;

            audioSource.Play();
        }

        private AudioSource GetSource()
        {
            if (audioSources == null)
            {
                audioSources = new List<AudioSource>();
                return NewSource();
            }

            foreach (var source in audioSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return NewSource();
        }

        private AudioSource NewSource()
        {
            GameObject go = new GameObject();
            go.name = "Audio Source";
            go.transform.SetParent(transform);

            var audioSource = go.AddComponent<AudioSource>();

            audioSources.Add(audioSource);

            return audioSource;
        }

        static public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
        {
            instance._PlaySound(clip, volume, pitch);
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) SoundManager.GetRawDescription";
        }

        public override string UpdateExplanation()
        {
            if (description != "") _explanation = description;
            else _explanation = "";

            return _explanation;
        }
    }
}