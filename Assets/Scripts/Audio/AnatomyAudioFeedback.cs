using System;
using UnityEngine;

namespace HumanAnatomy
{
    public class AnatomyAudioFeedback : MonoBehaviour
    {
        public static AnatomyAudioFeedback Instance { get; private set; }

        [Header("Audio Settings")]
        [Range(0f, 1f)] public float masterVolume = 0.65f;
        public bool isMuted = false;

        private AudioSource audioSource;
        private AudioClip hoverClip;
        private AudioClip selectClip;
        private AudioClip filterClip;
        private AudioClip explodeClip;
        private AudioClip whooshClip;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D UI sound

            GenerateProceduralAudioClips();
        }

        private void GenerateProceduralAudioClips()
        {
            hoverClip = CreateSineChime("SFX_Hover", 880f, 1320f, 0.05f, 0.25f);
            selectClip = CreateHarmonicBell("SFX_Select", 523.25f, 659.25f, 0.28f, 0.5f);
            filterClip = CreateTactileClick("SFX_Filter", 0.035f, 0.4f);
            explodeClip = CreateGlideWhoosh("SFX_Explode", 240f, 60f, 0.38f, 0.45f);
            whooshClip = CreateGlideWhoosh("SFX_Whoosh", 180f, 320f, 0.25f, 0.35f);
        }

        #region Sound Trigger Methods
        public void PlayHoverSound()
        {
            if (isMuted || hoverClip == null) return;
            audioSource.PlayOneShot(hoverClip, masterVolume * 0.4f);
        }

        public void PlaySelectSound()
        {
            if (isMuted || selectClip == null) return;
            audioSource.PlayOneShot(selectClip, masterVolume * 0.85f);
        }

        public void PlayFilterSound()
        {
            if (isMuted || filterClip == null) return;
            audioSource.PlayOneShot(filterClip, masterVolume * 0.6f);
        }

        public void PlayExplodeSound()
        {
            if (isMuted || explodeClip == null) return;
            audioSource.PlayOneShot(explodeClip, masterVolume * 0.7f);
        }

        public void PlayWhooshSound()
        {
            if (isMuted || whooshClip == null) return;
            audioSource.PlayOneShot(whooshClip, masterVolume * 0.5f);
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;
        }

        public void SetVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
        }
        #endregion

        #region Procedural Synthesis Generators
        private AudioClip CreateSineChime(string name, float startFreq, float endFreq, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, progress);
                float envelope = Mathf.Exp(-progress * 6.5f); // Fast exponential decay
                samples[i] = Mathf.Sin(2f * Mathf.PI * currentFreq * t) * envelope * volume;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateHarmonicBell(string name, float freq1, float freq2, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 4.5f);
                float wave1 = Mathf.Sin(2f * Mathf.PI * freq1 * t);
                float wave2 = Mathf.Sin(2f * Mathf.PI * freq2 * t) * 0.5f;
                float wave3 = Mathf.Sin(2f * Mathf.PI * (freq1 * 2f) * t) * 0.25f;
                samples[i] = (wave1 + wave2 + wave3) * env * volume * 0.6f;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateTactileClick(string name, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];
            System.Random rand = new System.Random();

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 15f);
                float noise = ((float)rand.NextDouble() * 2f - 1f) * 0.5f;
                float tone = Mathf.Sin(2f * Mathf.PI * 650f * t);
                samples[i] = (tone * 0.6f + noise * 0.4f) * env * volume;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateGlideWhoosh(string name, float startFreq, float endFreq, float duration, float volume)
        {
            int sampleRate = 44100;
            int sampleCount = Mathf.CeilToInt(sampleRate * duration);
            float[] samples = new float[sampleCount];
            System.Random rand = new System.Random();

            for (int i = 0; i < sampleCount; i++)
            {
                float t = (float)i / sampleRate;
                float progress = t / duration;
                float currentFreq = Mathf.Lerp(startFreq, endFreq, progress);
                float env = Mathf.Sin(progress * Mathf.PI); // Smooth rise and fall
                float tone = Mathf.Sin(2f * Mathf.PI * currentFreq * t);
                float noise = ((float)rand.NextDouble() * 2f - 1f) * 0.25f;
                samples[i] = (tone + noise) * env * volume * 0.7f;
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
        #endregion
    }
}

