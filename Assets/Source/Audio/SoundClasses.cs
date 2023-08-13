using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

namespace Cardificer
{


    /// <summary>
    /// The base of the Sound and SoundContainer class (Eventually the Music class to). Sounds allow for greater audio flexibility.
    /// </summary>
    [System.Serializable]
    public abstract class SoundBase
    {
        public abstract string name { get; }
        public AudioMixerGroup outputAudioMixerGroup; //will allow for better audio settings in the future
        public bool useDefaultSettings; //default settings found on AudioManager class
        public SoundSettings soundSettings;
        [HideInInspector] public AudioSource audioSourceInUse;

        /// <summary>
        /// Checks if this SoundBase is playing.
        /// </summary>
        public abstract bool IsPlaying();

        /// <summary>
        /// Stops this SoundBase. 
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Destroys the GameObject the AudioSource used by this SoundBase is attached to.
        /// </summary>
        public abstract void DestroyObject();
    }

    /// <summary>
    /// Meant for playing Oneshot style SFX
    /// </summary>
    [System.Serializable]
    public class Sound : SoundBase
    {

        [Header("Sound Settings")]
        public AudioClip audioClip;
        public override string name { get { return audioClip.name; } }

        /// <summary>
        /// Stops this Sound. 
        /// </summary>
        public override void Stop()
        {
            audioSourceInUse.Stop();
        }

        /// <summary>
        /// Checks if this Sound is playing.
        /// </summary>
        public override bool IsPlaying()
        {
            if (audioSourceInUse != null)
                return audioSourceInUse.isPlaying;
            else
                return false;
        }

        /// <summary>
        /// Destroys the GameObject attached to the AudioSource used by this Sound.
        /// </summary>
        public override void DestroyObject()
        {
            if (audioSourceInUse != null)
                Object.Destroy(audioSourceInUse.gameObject);
        }

    }

    /// <summary>
    /// Meant to play complicated sequences of SFX. 
    /// </summary>
    [System.Serializable]
    public class SoundContainer : SoundBase
    {
        [Header("Sound Container Settings")]
        [SerializeField] private string containerName = "NewSoundContainer";
        public AudioClip[] sounds;
        public override string name { get { return containerName; } }
        public SoundContainerType containerType;
        public bool loopContainer = false;
        public bool isPlaying;
        [HideInInspector] public bool shouldPlay;
        [HideInInspector] public IEnumerator coroutinePlayingThisContainer;


        /// <summary>
        /// Checks if this SoundContainer is playing.
        /// </summary>
        public override bool IsPlaying()
        {
            return isPlaying;
        }

        /// <summary>
        /// Stops this SoundContainer. 
        /// </summary>
        public override void Stop()
        {
            isPlaying = false;
            shouldPlay = false;
            if (audioSourceInUse != null)
                audioSourceInUse.Stop();
        }

        /// <summary>
        /// Destroys the GameObject attached to the AudioSource used by this SoundContainer.
        /// </summary>
        public override void DestroyObject()
        {
            if (audioSourceInUse != null)
                Object.Destroy(audioSourceInUse.gameObject);
        }


    }

    /// <summary>
    /// The settings for Sounds, SoundContainers, and eventually Music 
    /// </summary>
    [System.Serializable]
    public class SoundSettings
    {
        //Random values/ranges will not be applied if the corresponding bools are not true

        [Header("General Settings")]
        [Range(0, 256)] public int priority = 128;
        public bool loop;
        public bool destroyOnComplete;

        [Header("Pitch Settings")]
        [Range(0.001f, 1)] public float volume = 1;
        public bool randomizeVolume;
        public Vector2 volumeRandomRange = new Vector2(.8f, 1f);

        [Header("Volume Settings")]
        [Range(0.001f, 3)] public float pitch = 1;
        public bool randomizePitch;
        public Vector2 pitchRandomRange = new Vector2(.8f, 1.1f);

        [Header("Misc Settings")]
        [Range(0, 1)] public float spatialBlend = 0.5f;
        //[SerializeField] private bool _ignorePause;

    }

    /// <summary>
    /// Describes the playback type of a SoundContainer.
    /// </summary>
    public enum SoundContainerType
    {
        Sequential, //plays through each sound in the container from first -> last
        RandomSequential, //plays through each sound in the container randomly, but never playing each sound more than once per loop
        RandomRandom, //plays through each sound in the container, not caring if a sound plays more than once per loop
        RandomOneshot, //plays only one random AudioClip in the SoundContainer
        //RandomBurst, //for the future?
    }
}
