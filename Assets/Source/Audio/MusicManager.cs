using Cardificer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

    public class MusicManager : MonoBehaviour
    {
        public static MusicManager instance;

        public MusicSound ambientMusic, mainMenuMusic, battleMusic, bossMusic;
        public AudioMixerGroup masterAudioMixerGroup;
        public float speedUpDuration, slowDownDuration, durationToFadeOutChuggingAmbience, fadeOutChuggingAmbienceDuration;
        public int frameDelaySetPoint;
        public float ambiMixerGroupDefaultVolume, battleMixerGroupDefaultVolume;

        private bool bossBattle = false;


        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("There's more than one MusicManager! " + transform + " - " + instance);
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);
            transform.position = new Vector3(0, 0, 0);

        }

        private void Start()
        {
            StartCoroutine(FadeInAudio(0.05f));

        }


        private IEnumerator FadeInAudio(float fadeInDuration)
        {
            masterAudioMixerGroup.audioMixer.SetFloat("masterVolume", -80);
            float curTime = 0;

            while (curTime < fadeInDuration)
            {

                masterAudioMixerGroup.audioMixer.SetFloat("masterVolume", Mathf.Lerp(-80, 0, curTime / fadeInDuration));
                curTime += Time.deltaTime;
                yield return null;

            }

            masterAudioMixerGroup.audioMixer.SetFloat("masterVolume", 0);

        }

        public void StartMainMenuMusic()
        {
            StartCoroutine(FadeInTrack(mainMenuMusic, 0.2f));

            StartCoroutine(FadeOutTrack(mainMenuMusic, 1f, true));
            StartCoroutine(FadeOutTrack(battleMusic, 1f, true));
            StartCoroutine(FadeOutTrack(bossMusic, 1f, true));
        }

        public void StartLevelMusic()
        {
            StartCoroutine(FadeInTrack(ambientMusic, 0.5f));
            StartCoroutine(FadeOutTrack(mainMenuMusic, 0.5f, true));
        }

        public void SetMusicToAmbientState_public()
        {

            bossBattle = false;

            StartCoroutine(FadeInTrack(ambientMusic, 0.5f));
            StartCoroutine(FadeOutTrack(battleMusic, 0.5f, false));
            StartCoroutine(FadeOutTrack(bossMusic, 0.5f, true));
        }

        public void SetMusicToBattleState_public()
        {
            if (bossBattle) return;

            StartCoroutine(FadeOutTrack(ambientMusic, 0.5f, false));
            StartCoroutine(FadeInTrack(battleMusic, 0.5f));
            StartCoroutine(FadeOutTrack(bossMusic, 0.5f, true));
        }

        public void SetMusicToBossMusic_public()
        {
            bossBattle = true;
            StartCoroutine(FadeOutTrack(ambientMusic, 0.5f, false));
            StartCoroutine(FadeOutTrack(battleMusic, 0.5f, false));
            StartCoroutine(FadeInTrack(bossMusic, 0.5f));

        }


        private void ApplySoundSettingsToAudioSource(MusicSound sound, AudioSource audioSource)
        {

            audioSource.clip = sound.audioClip;
            audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;
            sound.audioSourceInUse = audioSource;

            audioSource.priority = sound.soundSettings.priority;
            audioSource.loop = sound.soundSettings.loop;
            //audioSource.volume = sound.GetVolume();
            audioSource.pitch = sound.GetPitch();
            audioSource.spatialBlend = 0;
            audioSource.spread = sound.soundSettings.spread;
        }

        private IEnumerator FadeOutTrack(SoundBase musicToFadeOut, float fadeOutDuration, bool shouldStop)
        {

            if (musicToFadeOut.audioSourceInUse == null || musicToFadeOut.audioSourceInUse.volume == 0f) yield break;

            float curTime = 0;

            while (curTime < fadeOutDuration)
            {

                musicToFadeOut.audioSourceInUse.volume = Mathf.Lerp(musicToFadeOut.GetVolume(), 0, curTime / fadeOutDuration);
                curTime += Time.deltaTime;
                yield return null;

            }

            musicToFadeOut.audioSourceInUse.volume = 0f;

            if (shouldStop)
            {
                musicToFadeOut.audioSourceInUse.Stop();
                AudioManager.instance.audioSourcesToDestroy.Add(musicToFadeOut.audioSourceInUse);

            }

        }

        private IEnumerator FadeInTrack(MusicSound musicToFadeIn, float fadeInDuration)
        {
            float curTime = 0;
            if (musicToFadeIn.audioSourceInUse == null) musicToFadeIn.audioSourceInUse = gameObject.AddComponent<AudioSource>();
            ApplySoundSettingsToAudioSource(musicToFadeIn, musicToFadeIn.audioSourceInUse);
            musicToFadeIn.audioSourceInUse.volume = 0;
            musicToFadeIn.audioSourceInUse.Play();


            while (curTime < fadeInDuration)
            {

                musicToFadeIn.audioSourceInUse.volume = Mathf.Lerp(0, musicToFadeIn.GetVolume(), curTime / fadeInDuration);
                curTime += Time.deltaTime;
                yield return null;

            }

            musicToFadeIn.audioSourceInUse.volume = musicToFadeIn.GetVolume();

        }

    }


