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

    public MusicSound[] ambientTracks, battleTracks;
    private bool musicSpeedChanging = false, musicSpeedUp = false, musicSpeedDown = false;
    private int musicSpeed = 0;
    private List<AudioSource> activeMusicAudioSources = new List<AudioSource>();
    public AudioMixerGroup ambientAudioMixerGroup;
    public AudioMixerGroup battleAudioMixerGroup;
    public AudioMixerGroup bossAudioMixerGroup;
    public float speedUpDuration, slowDownDuration, durationToFadeOutChuggingAmbience, fadeOutChuggingAmbienceDuration;
    private float pitchChangeAmountTargetUp = 1.222222222222222222222222222222f, pitchChangeAmountTargetDown = 0.81818181818181818181818f;
    public int frameDelaySetPoint;

    public MusicSound mainMenuMusic, bossMusic;
    private bool levelStarted = false;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There's more than one MusicManager! " + transform + " - " + instance);
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject); 
        transform.position = new Vector3(0, 0, 0);

        //ambientAudioMixerGroup.audioMixer.SetFloat("ambiVolume", ambiMixerGroupDefaultVolume);
        //battleAudioMixerGroup.audioMixer.SetFloat("battleVolume", battleMixerGroupDefaultVolume);

    }

    private void Start()
    {
        //foreach (MusicSound ambMusicSound in ambientTracks) 
        //{ 
        //    ambMusicSound.SetAudioMixerGroup(ambientAudioMixerGroup);
        //    AudioManager.instance.PlaySoundBaseOnTarget(ambMusicSound, this.gameObject.transform, true);
        //}

        //foreach (MusicSound battleMusicSound in battleTracks)
        //{
        //    battleMusicSound.SetAudioMixerGroup(battleAudioMixerGroup);
        //    AudioManager.instance.PlaySoundBaseOnTarget(battleMusicSound, this.gameObject.transform, true);
        //    battleMusicSound.audioSourceInUse.pitch = pitchChangeAmountTargetDown;
        //    battleAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", pitchChangeAmountTargetUp);
        //    battleMusicSound.audioSourceInUse.volume = 0;
        //}

    }

    private void Update()
    {
        //if (!musicSpeedChanging && Input.GetKeyDown(KeyCode.P))
        //{

        //    if (musicSpeed == 0)
        //    {
        //        StartCoroutine(SetMusicBattleState(speedUpDuration));

        //    }
        //    else if (musicSpeed == 1)
        //    {

        //        StartCoroutine(SetMusicAmbientState(slowDownDuration));

        //    }
        //}
    }

    public void SetMusicToBossMusic_public()
    {

        StartCoroutine(SetMusicToBossMusic());

    }

    private IEnumerator SetMusicToBossMusic()
    {

        float fadeOutTime = 0.5f;

        foreach (MusicSound musicSound in battleTracks)
        {
            StartCoroutine(FadeOutTrack(musicSound, fadeOutTime, false));
        }

        foreach (MusicSound musicSound in ambientTracks)
        {
            StartCoroutine(FadeOutTrack(musicSound, fadeOutTime, false));
        }

        yield return new WaitForSeconds(fadeOutTime);

        StartMusicTracksAtZeroVolume(bossMusic);
        StartCoroutine(FadeInTrack(bossMusic, 0.3f));
        battleAudioMixerGroup.audioMixer.SetFloat("battleVolume", -80);
        

    }

    public void SetMusicToAmbientState_public()
    {
        if (!levelStarted) return;


        foreach (MusicSound musicSound in battleTracks)
        {
            StartCoroutine(FadeOutTrack(musicSound, 1.3f, false));
        }

        foreach (MusicSound musicSound in ambientTracks)
        {
            StartCoroutine(FadeInTrack(musicSound, 1.3f));
        }

    }

    public void SetMusicToBattleState_public()
    {
        foreach (MusicSound musicSound in battleTracks)
        {
            StartCoroutine(FadeInTrack(musicSound, 1.3f));
        }

        foreach (MusicSound musicSound in ambientTracks)
        {
            StartCoroutine(FadeOutTrack(musicSound, 1.3f, false));
        }
    }

    public void StartLevelMusic()
    {

        StartCoroutine(FadeOutTrack(mainMenuMusic, 1.5f, true));
        StartMusicTracksAtZeroVolume(ambientTracks, battleTracks);
        foreach (MusicSound musicSound in ambientTracks)
        {
            StartCoroutine(FadeInTrack(musicSound, 2f));
        }

        StartCoroutine(WaitForRoomsToLoad());


        //foreach (MusicSound ambMusicSound in ambientTracks)
        //{
        //    ambMusicSound.SetAudioMixerGroup(ambientAudioMixerGroup);
        //    AudioManager.instance.PlaySoundBaseOnTarget(ambMusicSound, this.gameObject.transform, true);
        //}

        //foreach (MusicSound battleMusicSound in battleTracks)
        //{
        //    battleMusicSound.SetAudioMixerGroup(battleAudioMixerGroup);
        //    AudioManager.instance.PlaySoundBaseOnTarget(battleMusicSound, this.gameObject.transform, true, 0);
        //    battleMusicSound.audioSourceInUse.pitch = pitchChangeAmountTargetDown;
        //    battleAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", pitchChangeAmountTargetUp);
        //    battleMusicSound.audioSourceInUse.volume = 0;
        //}

        //StartCoroutine(FadeOutChuggingAmbientTrack());

    }

    private IEnumerator WaitForRoomsToLoad()
    {
        yield return new WaitForSeconds(1);

        levelStarted = true;
    }

    private void StartMusicTracksAtZeroVolume(MusicSound[] ambientMusicTracks, MusicSound[] battleMusicTracks)
    {

        foreach (MusicSound track in ambientMusicTracks)
        {

            track.audioSourceInUse = this.gameObject.AddComponent<AudioSource>();
            ApplySoundSettingsToAudioSource(track, track.audioSourceInUse);
            track.audioSourceInUse.volume = 0f;
            track.audioSourceInUse.Play();
        }

        foreach (MusicSound track in battleMusicTracks)
        {

            track.audioSourceInUse = this.gameObject.AddComponent<AudioSource>();
            ApplySoundSettingsToAudioSource(track, track.audioSourceInUse);
            track.audioSourceInUse.volume = 0f;
            track.audioSourceInUse.Play();
        }

    }

    private void StartMusicTracksAtZeroVolume(MusicSound bossMusicTrack)
    {
        bossMusicTrack.audioSourceInUse = this.gameObject.AddComponent<AudioSource>();
        ApplySoundSettingsToAudioSource(bossMusicTrack, bossMusicTrack.audioSourceInUse);
        bossMusicTrack.audioSourceInUse.volume = 0f;
        bossMusicTrack.audioSourceInUse.Play();
    }


    private void ApplySoundSettingsToAudioSource(MusicSound sound, AudioSource audioSource)
    {

        audioSource.clip = sound.audioClip;
        audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;
        sound.audioSourceInUse = audioSource;

        audioSource.priority = sound.soundSettings.priority;
        audioSource.loop = sound.soundSettings.loop;
        audioSource.volume = sound.GetVolume();
        audioSource.pitch = sound.GetPitch();
        audioSource.spatialBlend = sound.soundSettings.spatialBlend;
        audioSource.spread = sound.soundSettings.spread;
    }

    private IEnumerator FadeOutTrack(SoundBase musicToFadeOut, float fadeOutDuration, bool shouldStop)
    {

        if (musicToFadeOut.audioSourceInUse == null || musicToFadeOut.audioSourceInUse.volume == 0f) yield break;

        float curTime = 0;

        while (curTime < fadeOutDuration)
        { 
            
            musicToFadeOut.audioSourceInUse.volume = Mathf.Lerp(musicToFadeOut.soundSettings.volume, 0, curTime/fadeOutDuration);
            curTime += Time.deltaTime;
            yield return null;
        
        }

        if (shouldStop)
        { 
            musicToFadeOut.audioSourceInUse.Stop();
            AudioManager.instance.audioSourcesToDestroy.Add(musicToFadeOut.audioSourceInUse);

        }

    }

    private IEnumerator FadeInTrack (MusicSound musicToFadeIn, float fadeInDuration)
    {
        float curTime = 0;

        while (curTime < fadeInDuration)
        {

            musicToFadeIn.audioSourceInUse.volume = Mathf.Lerp(0, musicToFadeIn.soundSettings.volume, curTime / fadeInDuration);
            curTime += Time.deltaTime;
            yield return null;

        }

    }

    private IEnumerator SetMusicBattleState(float fadeToBattleTime)
    {

        //StopAllCoroutines();

        musicSpeedChanging = true;
        float curTime = 0;
        int frameDelay = 0;

        while (curTime < fadeToBattleTime) 
        {

            //Speed up and fade out ambiences----------------------------
            float curAmbiPitchChangeAmount = Mathf.Lerp(1, pitchChangeAmountTargetUp, curTime/fadeToBattleTime);
            
            foreach (MusicSound musicSound in ambientTracks)
            {
                musicSound.audioSourceInUse.pitch = curAmbiPitchChangeAmount;

                float volume = Mathf.Lerp(musicSound.GetVolume(), 0, curTime/fadeToBattleTime);
                musicSound.audioSourceInUse.volume = volume;

            }

            ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1f / curAmbiPitchChangeAmount);

            if (frameDelay > frameDelaySetPoint)
            {
                ////Speed up and fade in battle music-----------------------
                float curBattlePitchChangeAmount = Mathf.Lerp(pitchChangeAmountTargetDown, 1, curTime / fadeToBattleTime);

                foreach (MusicSound battleMusic in battleTracks)
                {
                    battleMusic.audioSourceInUse.pitch = curBattlePitchChangeAmount;

                    float volume = Mathf.Lerp(0, battleMusic.GetVolume(), curTime / fadeToBattleTime);
                    battleMusic.audioSourceInUse.volume = volume;

                }

                ambientAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", 1f / curBattlePitchChangeAmount);

            }

            curTime += Time.deltaTime;
            frameDelay += 1;
            yield return null;

        }

        //"Clamp" values
        //Ambient tracks (sped up and muted)
        foreach (MusicSound ambiMusicSound in ambientTracks)
        {
            ambiMusicSound.audioSourceInUse.pitch = pitchChangeAmountTargetUp;
            ambiMusicSound.audioSourceInUse.volume = 0;
        }

        ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1f / pitchChangeAmountTargetUp);

        //Battle tracks (Normal speed and full volume)
        foreach (MusicSound battleMusicSound in battleTracks)
        {
            battleMusicSound.audioSourceInUse.pitch = 1;
            battleMusicSound.audioSourceInUse.volume = battleMusicSound.GetVolume();
        }

        battleAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", 1);

        musicSpeedChanging = false;
        musicSpeed = 1;

        yield break;

    }

    private IEnumerator FadeOutChuggingAmbientTrack()
    {
        bool finished = false;
        float curTime = 0;
        float fadingTime = 0;
        MusicSound chuggingAmbience = ambientTracks[1];

        while (!finished)
        {

            if (curTime < durationToFadeOutChuggingAmbience)
            {
                curTime += Time.deltaTime;
                yield return null;

            } else if (curTime > durationToFadeOutChuggingAmbience)
            {
                             

                fadingTime = curTime - durationToFadeOutChuggingAmbience;
                float volume = Mathf.Lerp(chuggingAmbience.GetVolume(), 0, fadingTime / fadeOutChuggingAmbienceDuration);

                chuggingAmbience.audioSourceInUse.volume = volume;
                curTime += Time.deltaTime;

                if (fadingTime > fadeOutChuggingAmbienceDuration)
                {
                    finished = true;
                }

                yield return null;

            }

            //print("curTime = " + curTime + ".\nfadingTime = " + fadingTime + "\nVolume = " + chuggingAmbience.audioSourceInUse.volume);

        }

        chuggingAmbience.audioSourceInUse.volume = 0;
        //print("done");

    }

    private IEnumerator SetMusicAmbientState(float fadeToAmbienceTime)
    {
        StartCoroutine(FadeOutChuggingAmbientTrack());
        //StopAllCoroutines();
        musicSpeedChanging = true;
        float curTime = 0;
        int frameDelay = 0;

        while (curTime < fadeToAmbienceTime)
        {            

            if (frameDelay > frameDelaySetPoint)
            {
                //Slow down and fade in ambiences----------------------------
                float curAmbiPitchChangeAmount = Mathf.Lerp(pitchChangeAmountTargetUp, 1, curTime / fadeToAmbienceTime);

                foreach (MusicSound musicSound in ambientTracks)
                {
                    musicSound.audioSourceInUse.pitch = curAmbiPitchChangeAmount;

                    float volume = Mathf.Lerp(0, musicSound.GetVolume(), curTime / fadeToAmbienceTime);
                    musicSound.audioSourceInUse.volume = volume;

                    //print(volume);

                }

                ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1f / curAmbiPitchChangeAmount);
            }

            ////Slow down and fade out battle music-----------------------
                        float curBattlePitchChangeAmount = Mathf.Lerp(1, pitchChangeAmountTargetDown, curTime / fadeToAmbienceTime);

            foreach (MusicSound battleMusic in battleTracks)
            {
                battleMusic.audioSourceInUse.pitch = pitchChangeAmountTargetDown;

                float volume = Mathf.Lerp(battleMusic.GetVolume(), 0, curTime / fadeToAmbienceTime);
                battleMusic.audioSourceInUse.volume = volume;

            }

            ambientAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", 1f / pitchChangeAmountTargetDown);

            curTime += Time.deltaTime;
            frameDelay += 1;
            yield return null;

        }

        //"Clamp" values
        //Ambient tracks (Normal speed and full volume)
        foreach (MusicSound ambiMusicSound in ambientTracks)
        {
            ambiMusicSound.audioSourceInUse.pitch = 1;
            ambiMusicSound.audioSourceInUse.volume = ambiMusicSound.GetVolume();
        }

        ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1);

        //Battle tracks (Slowed down and muted)
        foreach (MusicSound battleMusicSound in battleTracks)
        {
            battleMusicSound.audioSourceInUse.pitch = pitchChangeAmountTargetDown;
            battleMusicSound.audioSourceInUse.volume = 0;
        }

        battleAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", 1f / pitchChangeAmountTargetDown);

        musicSpeedChanging = false;
        musicSpeed = 0;

        yield break;

    }

    internal void StartMainMenuMusic()
    {
        AudioManager.instance.PlaySoundBaseOnTarget(mainMenuMusic, this.transform, true);
    }

    public void SetMusicToAmbientMusicFromBossMusic_public()
    {
        SetMusicToAmbientMusicFromBossMusic();
    }

    private void SetMusicToAmbientMusicFromBossMusic()
    {
        StartCoroutine(FadeOutTrack(bossMusic, 1, true));
        foreach (MusicSound track in ambientTracks)
        {
            FadeInTrack(track, 1);
        }
        StartCoroutine(FadeInBattleMixer());

        IEnumerator FadeInBattleMixer()
        {
            yield return new WaitForSeconds(1);
            battleAudioMixerGroup.audioMixer.SetFloat("battleVolume", 0);
        }


    }

}
