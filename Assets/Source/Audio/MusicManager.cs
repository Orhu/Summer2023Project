using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public float speedUpDuration, slowDownDuration;
    private float pitchChangeAmountTargetUp = 1.222222222222222222222222222222f, pitchChangeAmountTargetDown = 0.81818181818181818181818f;


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

        //ambientAudioMixerGroup.audioMixer.SetFloat("ambiVolume", ambiMixerGroupDefaultVolume);
        //battleAudioMixerGroup.audioMixer.SetFloat("battleVolume", battleMixerGroupDefaultVolume);

    }

    private void Start()
    {
        foreach (MusicSound ambMusicSound in ambientTracks) 
        { 
            ambMusicSound.SetAudioMixerGroup(ambientAudioMixerGroup);
            AudioManager.instance.PlaySoundBaseOnTarget(ambMusicSound, this.gameObject.transform, true);
        }

        foreach (MusicSound battleMusicSound in battleTracks)
        {
            battleMusicSound.SetAudioMixerGroup(battleAudioMixerGroup);
            battleAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", pitchChangeAmountTargetUp);
            AudioManager.instance.PlaySoundBaseOnTarget(battleMusicSound, this.gameObject.transform, true);
            battleMusicSound.audioSourceInUse.pitch = pitchChangeAmountTargetDown;
            battleMusicSound.audioSourceInUse.volume = 0;
        }

    }

    private void Update()
    {
        if (!musicSpeedChanging && Input.GetKeyDown(KeyCode.P))
        {

            if (musicSpeed == 0)
            {
                StartCoroutine(SetMusicBattleState(speedUpDuration));

            }
            else if (musicSpeed == 1)
            {

                StartCoroutine(SetMusicAmbientState(slowDownDuration));

            }
        }
    }

    private IEnumerator SetMusicBattleState(float fadeToBattleTime)
    {
        musicSpeedChanging = true;
        float curTime = 0; 

        while (curTime < fadeToBattleTime) 
        {

            //Speed up and fade out ambiences----------------------------
            float curAmbiPitchChangeAmount = Mathf.Lerp(1, pitchChangeAmountTargetUp, curTime/fadeToBattleTime);
            
            foreach (MusicSound musicSound in ambientTracks)
            {
                musicSound.audioSourceInUse.pitch = curAmbiPitchChangeAmount;

                float volume = Mathf.Lerp(musicSound.GetVolume(), 0, curTime/fadeToBattleTime);
                musicSound.audioSourceInUse.volume = volume;

                print (volume);

            }

            ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1f / curAmbiPitchChangeAmount);

            ////Speed up and fade in battle music-----------------------
            float curBattlePitchChangeAmount = Mathf.Lerp(pitchChangeAmountTargetDown, 1, curTime / fadeToBattleTime);

            foreach (MusicSound battleMusic in battleTracks)
            {
                battleMusic.audioSourceInUse.pitch = curBattlePitchChangeAmount;

                float volume = Mathf.Lerp(0, battleMusic.GetVolume(), curTime / fadeToBattleTime);
                battleMusic.audioSourceInUse.volume = volume;

            }

            ambientAudioMixerGroup.audioMixer.SetFloat("battlePitchBend", 1f / curBattlePitchChangeAmount);

            curTime += Time.deltaTime;
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

    private IEnumerator SetMusicAmbientState(float fadeToAmbienceTime)
    {
        musicSpeedChanging = true;
        float curTime = 0;

        while (curTime < fadeToAmbienceTime)
        {

            //Slow down and fade in ambiences----------------------------
            float curAmbiPitchChangeAmount = Mathf.Lerp(pitchChangeAmountTargetUp, 1, curTime / fadeToAmbienceTime);

            foreach (MusicSound musicSound in ambientTracks)
            {
                musicSound.audioSourceInUse.pitch = curAmbiPitchChangeAmount;

                float volume = Mathf.Lerp(0, musicSound.GetVolume(), curTime / fadeToAmbienceTime);
                musicSound.audioSourceInUse.volume = volume;

                print(volume);

            }

            ambientAudioMixerGroup.audioMixer.SetFloat("ambiPitchBend", 1f / curAmbiPitchChangeAmount);

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


}
