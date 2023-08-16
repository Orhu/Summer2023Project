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

        [Header("Sound Base Settings")]

        [Tooltip("The Mixer Group this SoundBase should be assigned to.")]
        public AudioMixerGroup outputAudioMixerGroup; //will allow for better audio settings in the future

        [Tooltip("Set if this SoundBase should use the default SoundSettings. The default SoundSettings are found on the AudioManager class.")]

        //The name of this SoundBase
        public abstract string name { get; }

        public bool useDefaultSettings = true;

        [Tooltip("The SoundSettings used by this SoundBase. These settings are ignored if Use Default Settings is true.")]
        public SoundSettings soundSettings;

        //The AudioSource used by this SoundBase
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
    /// Meant for playing Oneshot style SFX.
    /// </summary>
    [System.Serializable]
    public class Sound : SoundBase
    {

        [Header("Sound Settings")]
        [Tooltip("The AudioClip played by this Sound.")]
        public AudioClip audioClip;

        //Returns the name of the AudioClip played by this sound.
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
    /// Used to play complicated sequences of SFX. 
    /// </summary>
    [System.Serializable]
    public class SoundContainer : SoundBase
    {
        [Header("Sound Container Settings")]

        [Tooltip("The name of this SoundContainer. Used to describe the sounds within this SoundContainer.")]
        [SerializeField] private string containerName = "NewSoundContainer";

        [Tooltip("The AudioClips used in this container.")]
        public AudioClip[] clipsInContainer;

        //Returns the name set in the inspector
        public override string name { get { return containerName; } }

        [Tooltip("The playback type of this SoundContainer.")]
        public SoundContainerType containerType;

        [Tooltip("Set whether the SoundContainer should loop once the playback is completed.")]
        public bool loopContainer = false;
        
        //Stops this SoundContainer from being prematurely deleted by the DestroyExpiredAudio method in the AudioManager class
        [HideInInspector] public bool isPlaying;

        //Checked by the SoundContainer to see if it should stop playing
        [HideInInspector] public bool shouldPlay;

        //Unused, meant to stop individual containers in the future
        //[HideInInspector] public IEnumerator coroutinePlayingThisContainer;


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
    /// The settings for Sounds, SoundContainers, and eventually Music. Random values/ranges will not be applied if the corresponding bools are not true.
    /// </summary>
    [System.Serializable]
    public class SoundSettings
    {

        [Header("General Settings")]

        [Tooltip("Sets the prioroity of the AudioSource. The lower the number the higher the priority when Unity decides which sounds to play or cull.")]
        [Range(0, 256)] public int priority = 128;

        [Tooltip("Set if the Sound should play again after playback.")]
        public bool loop;

        [Header("Volume Settings")]

        [Tooltip("Set the volume to play the Sound at. This setting will be ignored on playback if Randomize Volume is true.")]
        [Range(0.001f, 1)] public float volume = 1;

        [Tooltip("Set if the Volume of this Sound should be randomized.")]
        public bool randomizeVolume;

        [Tooltip("Set the range of volume randomization for this Sound. X value is the minimum volume, and the Y value is the max. These settings will be ignored on playback if Randomize Volume is false.")]
        public Vector2 volumeRandomRange = new Vector2(.8f, 1f);

        [Header("Pitch Settings")]

        [Tooltip("Set the pitch to play the Sound at. This setting will be ignored on playback if Randomize Pitch is true.")]
        [Range(0.001f, 3)] public float pitch = 1;

        [Tooltip("Set if the pitch of this Sound should be randomized.")]
        public bool randomizePitch;

        [Tooltip("Set the range of pitch randomization for this Sound. X value is the minimum pitch, and the Y value is the max. These settings will be ignored on playback if Randomize Pitch is false.")]
        public Vector2 pitchRandomRange = new Vector2(.8f, 1.1f);

        [Header("Misc Settings")]

        [Tooltip("Set the Spacial Blend for this Sound. 0 = 2D playback, 1 = full 3D playback.")]
        [Range(0, 1)] public float spatialBlend = 0.5f;
        //[SerializeField] private bool _ignorePause;

    }

    /// <summary>
    /// Describes the playback type of a SoundContainer.
    /// </summary>
    public enum SoundContainerType
    {
        Sequential, //plays through each AudioClip in the container from first -> last
        RandomSequential, //plays through each AudioClip in the container randomly, but never playing each AudioClip more than once per loop
        RandomRandom, //plays through each AudioClip in the container randomly, not caring if an AudioClip plays more than once per loop
        RandomOneshot, //plays only one random AudioClip in the SoundContainer
        //RandomBurst, //for the future?
    }
}
