using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Cardificer
{

    /// <summary>
    /// Controls the music playback for the game. Assumes you only have one source of non-diagetic music
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour
    {
        //Singleton pattern
        public static MusicController Instance;

        [Header("Music Setup")]
        [Tooltip("List of all music tracks to be played over the course of the game.")]
        public MusicTrack[] musicTracks;

        //Variables used during music playback
        private AudioSource audioSource;
        private MusicTrack curTrack;
        private AudioClip[] musicLayersToPlay;
        private bool shouldPlay = false;

        //Variables used for calculating the bpm of the music
        private double curBPM = 140.0F;
        private TimeSignature curTimeSignature;
        private int beatCounter = 0;
        private int loopLengthInBeats;

        [Header("Metronome Settings")]
        [Tooltip("Set if you want to hear the metronome or not.")]
        public bool muteMetronome = true;

        [Tooltip("Set loud the metronome is.")]
        [Range(0, 1)] public float gain = 0.5F;

        //Settings for setting the amplitude of the audio stream manually
        private double nextTick = 0.0F;
        private float amp = 0.0F;
        private float phase = 0.0F;
        private double sampleRate = 0.0F;

        //Tracking the beat
        private int curBeat;
        private bool beatRunning = true;

        [Header("Parameter Setup")]
        [Tooltip("List of all global FloatParameters that will affect the music.")]
        public FloatParameter[] globalFloatParameters;
        [Tooltip("List of all global BoolParameters that will affect the music.")]
        public BoolParameter[] globalBoolParameters;

        /// <summary>
        /// Implement singleton pattern and assign the main AudioSource
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one AudioManager!" + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            audioSource = GetComponent<AudioSource>();

        }

        /// <summary>
        /// Initialize the first music track and beat management
        /// </summary>
        void Start()
        {
            //Initialize first MusicTrack
            InitializeMusicTrack(musicTracks[0]);
            LoadAudioClipsToPlay();

            //Initialize beat tracking
            curBeat = curTimeSignature.topValue;
            double startTick = AudioSettings.dspTime;
            sampleRate = AudioSettings.outputSampleRate;
            nextTick = startTick * sampleRate;

            //start playback
            PlayLoadedMusicLayers();
            beatRunning = true;

        }

        /// <summary>
        /// Check if new layers should be played, maybe move to fixed update or something less variable?
        /// </summary>
        private void Update()
        {
            if (shouldPlay && beatRunning)
            {
                PlayLoadedMusicLayers();
            }
        }

        /// <summary>
        /// Monitors the output of the audio source, using dspTime to calculate when to play the next clips
        /// </summary>
        /// <param name="data">An array of floats comprising the audio data. </param>
        /// <param name="channels">An int that stores the number of channels of audio data passed to this delegate. </param>
        void OnAudioFilterRead(float[] data, int channels) //beat detection and when to play logic
        {
            if (!beatRunning)
                return;

            double samplesPerTick = sampleRate * 60.0F / curBPM * 4.0F / curTimeSignature.botValue;
            double sample = AudioSettings.dspTime * sampleRate;
            int dataLen = data.Length / channels;
            int n = 0;

            while (n < dataLen)
            {
                if (!muteMetronome) //metronome stuff
                {
                    float x = gain * amp * Mathf.Sin(phase);
                    int i = 0;
                    while (i < channels)
                    {
                        data[n * channels + i] += x;
                        i++;
                    }
                }

                while (sample + n >= nextTick)
                {

                    nextTick += samplesPerTick;
                    amp = 1.0F;

                    //Increment metronome count
                    if (++curBeat > curTimeSignature.topValue)
                    {
                        curBeat = 1;
                        amp *= 2.0F;
                    }

                    //if (++beatCounter == loopLengthInBeats)
                    //{
                    //    LoadAudioClipsToPlay();
                    //}

                    //increment beatCounter count and check if we should play new clips
                    if (++beatCounter > loopLengthInBeats)
                    {
                        LoadAudioClipsToPlay();
                        shouldPlay = true;
                        beatCounter = 1;
                    }

                    Debug.Log("Beat: " + curBeat + "/" + curTimeSignature.topValue + "\n" +
                              "Beat Counter = " + beatCounter);

                }
                if (!muteMetronome)
                {
                    phase += amp * 0.3F;
                    amp *= 0.993F;
                }
                n++;
            }
        }

        /// <summary>
        /// Sets the settings used by the beat tracker
        /// </summary>
        /// <param name="trackToInitialize">The MusicTrack to take the settings from. </param>>
        private void InitializeMusicTrack(MusicTrack trackToInitialize)
        {
            curBPM = trackToInitialize.bpm;
            curTimeSignature.topValue = trackToInitialize.timeSignature.topValue;
            curTimeSignature.botValue = trackToInitialize.timeSignature.botValue;
            loopLengthInBeats = trackToInitialize.loopLengthInBeats;

            curTrack = trackToInitialize;

        }

        /// <summary>
        /// Sets the layers to play next
        /// </summary>
        private void LoadAudioClipsToPlay()
        {
            musicLayersToPlay = curTrack.GetLayersToPlay();
        }

        /// <summary>
        /// Play the loaded layers
        /// </summary>
        private void PlayLoadedMusicLayers()
        {
            foreach (AudioClip ac in musicLayersToPlay)
            {
                //eventually will be able to play with volume control
                audioSource.PlayOneShot(ac);
            }

            shouldPlay = false;

        }

        /// <summary>
        /// Fades an AudioSource to a target volume.
        /// </summary>
        /// <param name="audioSourceToFade">The AUdioSource to fade the volume of. </param>
        /// <param name="duration">How long the fade lasts. </param>
        /// <param name="targetVolume">The volume to fade to. </param>
        /// <param name="stopOnEnd">Should the AudioSource stop once the fade has finished? </param>
        public IEnumerator FadeMusic(AudioSource audioSourceToFade, float duration, float targetVolume, bool stopOnEnd)
        {

            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                audioSourceToFade.volume = Mathf.Lerp(audioSourceToFade.volume, targetVolume, timeElapsed / duration);
                timeElapsed += Time.fixedDeltaTime;
                yield return null;
            }

            if (stopOnEnd && timeElapsed < duration)
            {
                audioSourceToFade.Stop();
            }

        }

    }

    /// <summary>
    /// Used to set up musical tracks. Each track is broken into layers and then when asked returns AudioClips that should be played based on set parameters
    /// </summary>
    [System.Serializable]
    public class MusicTrack
    {
        [Header("Track Setup")]
        [Tooltip("The name of this MusicTrack.")]
        public string name = "New Track";
        [Tooltip("The layers contained in this MusicTrack.")]
        public MusicLayer[] musicLayers;
        [Tooltip("How often this MusicTrack loops.")]
        public int loopLengthInBeats = 8;
        [Tooltip("The time signature of this MusicTrack.")]
        public TimeSignature timeSignature = new TimeSignature(4, 4);
        [Tooltip("The bpm of this MusicTrack.")]
        public int bpm = 100;

        [Header("Parameter Setup")]
        [Tooltip("The FloatParameters of this MusicTrack.")]
        public FloatParameter[] localFloatParameters;
        [Tooltip("The BoolParameters of this MusicTrack.")]
        public BoolParameter[] localBoolParameters;

        /// <summary>
        /// Calculate which layers should be looped
        /// </summary>
        /// <returns>The array of AudioClips that should be played on next loop</returns>
        public AudioClip[] GetLayersToPlay()
        {
            List<AudioClip> clipsToReturn = new List<AudioClip>();

            foreach (MusicLayer layer in musicLayers)
            {

                List<bool> boolList = new List<bool>() { true };

                //If a parameter is out of range then add false
                if (layer.floatParametersRanges != null)
                {
                    //check if global float parameters are outside layer range
                    foreach (FloatParameter fParam in MusicController.Instance.globalFloatParameters)
                    {
                        foreach (FloatParameterRange fRange in layer.floatParametersRanges)
                        {
                            if (fParam.name.Equals(fRange.name))
                            {
                                if (fParam.value < fRange.min || fParam.value > fRange.max)
                                {
                                    boolList.Add(!fRange.playInRange);
                                }
                                else
                                {
                                    boolList.Add(fRange.playInRange);
                                }
                            }
                        }
                    }

                    //check if local float parameters are outside layer range
                    foreach (FloatParameter fParam in localFloatParameters)
                    {
                        foreach (FloatParameterRange fRange in layer.floatParametersRanges)
                        {
                            if (fParam.name.Equals(fRange.name))
                            {
                                if (fParam.value < fRange.min || fParam.value > fRange.max)
                                {
                                    boolList.Add(!fRange.playInRange);
                                }
                                else
                                {
                                    boolList.Add(fRange.playInRange);
                                }
                            }
                        }
                    }
                }

                //If a bool is not equal then add false
                if (layer.boolParameters != null)
                {

                    //check global bool parameters
                    foreach (BoolParameter bParam in MusicController.Instance.globalBoolParameters)
                    {
                        foreach (BoolParameter bParamLayer in layer.boolParameters)
                        {
                            if (bParam.name.Equals(bParamLayer.name))
                            {
                                if (bParam.playValue == bParamLayer.playValue)
                                {
                                    boolList.Add(true);
                                }
                                else
                                {
                                    boolList.Add(false);
                                }
                            }
                        }
                    }

                    //check local bool parameters
                    foreach (BoolParameter bParam in localBoolParameters)
                    {
                        foreach (BoolParameter bParamLayer in layer.boolParameters)
                        {
                            if (bParam.name.Equals(bParamLayer.name))
                            {
                                if (bParam.playValue == bParamLayer.playValue)
                                {
                                    boolList.Add(true);
                                }
                                else
                                {
                                    boolList.Add(false);
                                }
                            }
                        }
                    }
                }

                //If at any time the layer should not be playing, do not add it to the return list
                if (boolList.Contains(false))
                {
                    continue;
                }
                else
                {
                    clipsToReturn.Add(layer.audioClip);
                }
            }

            //Return every layer that had no falses added to it's list
            return clipsToReturn.ToArray();

        }

    }

    /// <summary>
    /// Used to set up musical layers
    /// </summary>
    [System.Serializable]
    public class MusicLayer
    {
        [Tooltip("The name of this MusicLayer.")]
        public string layerName;
        [Tooltip("The AudioClip of this MusicLayer.")]
        public AudioClip audioClip;
        [Tooltip("The float ParameterRange of this MusicLayer.")]
        public FloatParameterRange[] floatParametersRanges;
        [Tooltip("The float ParameterRange of this MusicLayer.")]
        public BoolParameter[] boolParameters;

        //Yet to be implemented, set to control the volume or the playback bool per parameter
        //[Tooltip("The float ParameterRange of this MusicLayer.")]
        //public ParameterSetup[] parameterSetup;

    }

    #region music related enums and structs

    /* ! PARAMETER SYSTEM STRUCTS AND ENUMS BELOW !
     * 
     * The Parameter System works like this:
     *  - Every MusicLayer has Parameters that it uses to see if it should play
     *  - Parameters come in two types: BoolParameters and FloatParameters (currently)
     *  - Parameters of all types are made up of a Name and a Value
     *      - BoolParameter's value is a bool
     *      - FloatParameter's value is a float
     *  - Parameters can be set locally on a MusicTrack (for one MusicTrack) or globally on the MusicController (for every MusicTrack)
     *  - When the MusicController loads the next Layers to play, the MusicTrack compares the names of all local and global Parameters to the names of the Parameters on the MusicLayer
     *  - If a name matches then:
     *      - If the bool value of the Parameter matches the bool value on the MusicLayer, then the layer will play
     *      - If the float value of the Parameter falls within the range of the MusicLayer, then the layer checks if it should be playing or not and acts accordingly (see MusicLayer struct)
     *  - The MusicTrack only selects a layer to play if after all checks the layer should still play
     *  - The MusicTrack sends an array of all AudioClips (taken from selected layers) to the MusicController to play
     *  
     *  - FOR THE SYSTEM TO WORK ALL LOCAL AND GLOBAL PARAMETER NAMES MUST MATCH CORRESPONDING PARAMETER NAMES ON THE MUSICLAYER
     *  
     */

    /// <summary>
    /// Checked by MusicLayers to see if the Value here falls within their FloatParameterRange
    /// </summary>
    [System.Serializable]
    public struct FloatParameter
    {
        [Tooltip("The name of this FloatParameter.")]
        public string name;
        [Tooltip("The value of this FloatParameter.")]
        public float value;
    }

    /// <summary>
    /// Checked by MusicLayers to see if the PlayValue here equals the PlayValue in their own BoolParameter
    /// </summary>
    [System.Serializable]
    public struct BoolParameter
    {
        [Tooltip("The name of this BoolParameter.")]
        public string name;
        [Tooltip("The value of this BoolParameter.")]
        public bool playValue;
    }

    /// <summary>
    /// Used by MusicLayers to compare their range with a value set either locally (on the MusicTrack) or globally (on the MusicController)
    /// </summary>
    [System.Serializable]
    public struct FloatParameterRange
    {
        [Tooltip("The name of this FloatParameterRange. Should be set to the corresponding name of a FloatParameter")]
        public string name;
        [Tooltip("The minimum value of this FloatParameterRange.")]
        public int min;
        [Tooltip("The max value of this FloatParameterRange.")]
        public int max;
        [Tooltip("The playValue of this FloatParameterRange. If set to true, any value inside the range will cause this layer to play. If set to false, any value outside the range will cause this layer to play.")]
        public bool playInRange;
    }
    /// <summary>
    /// Not implemented yet
    /// </summary>
    [System.Serializable]
    public struct ParameterSetup
    {
        public string name;
        public AffectedValue affectedValue;
    }

    /// <summary>
    /// Is set on MusicTracks to tell the MusicController what time signature the music is in.
    /// </summary>
    [System.Serializable]
    public struct TimeSignature
    {
        [Tooltip("The top number in the time signature.")]
        public int topValue;
        [Tooltip("The bottom number in the time signature.")]
        public int botValue;

        //Constructor
        public TimeSignature(int topValue, int botValue)
        {
            this.topValue = topValue;
            this.botValue = botValue;
        }

    }

    /// <summary>
    /// Yet to be implemented, will control the volume or the playback bool per parameter
    /// </summary>
    public enum AffectedValue
    {
        VOLUME,
        PLAYBACK,
    }

    #endregion

}