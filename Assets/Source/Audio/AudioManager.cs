using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

namespace Cardificer
{
    /// <summary>
    /// Contains all the logic to playback SoundBases (SFX) and control general audio settings.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        //Singleton Pattern
        public static AudioManager instance;

        [Tooltip("Toggle AudioManager debug messages.")]
        public bool printDebugMessages;

        [Tooltip("Attach the AudioListener to the player or the AudioManager.")]
        public bool usePlayerAudioListener;

        private GameObject audioListenerGameObject;

        //Lists of objects to be destroyed or affected
        private List<SoundBase> soundsToDestroyList = new List<SoundBase>();
        //private List<AverageAudio> averageAudioList = new List<AverageAudio>();
        private List<SoundContainer> activeSoundContainers = new List<SoundContainer>();
        private List<AudioSource> audioSourcesToDestroy = new List<AudioSource>();

        [Tooltip("Default SoundSettings to be applied when a SoundBase has 'Use Default Settings' set to true.")]
        public SoundSettings _defaultSoundSettings;

        //Serialized Random for use in random SoundContainer playback
        private System.Random random = new System.Random();

        /// <summary>
        /// Implementing the singleton pattern and DontDestroyOnLoad. Creates the AudioListener GameObject. 
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("There's more than one AudioManager! " + transform + " - " + instance);
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            transform.position = new Vector3 (0,0,0); 

            audioListenerGameObject = new GameObject();
            audioListenerGameObject.name = "AudioListenerGameObject";
            audioListenerGameObject.AddComponent<AudioListener>();
            audioListenerGameObject.transform.SetParent(transform);

        }

        #region event subscription in OnEnable and OnDisable
        private void OnEnable()
        {

            SceneManager.sceneLoaded += OnSceneLoaded;

        }
        private void OnDisable()
        {

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        #endregion


        /// <summary>
        /// The event called when a Scene is loaded. 
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetUpAudioListeners();
        }

        /// <summary>
        /// Makes the AudioListener a child of the AudioManager so it doesn't get destroyed when a scene changes.
        /// </summary>
        public void ResetAudioListener()
        {
            audioListenerGameObject.transform.SetParent(transform);
        }

        /// <summary>
        /// Destroys all AudioListeners in a scene then attaches the AudioListener to the Player if usePlayerAudioListener is true.
        /// </summary>
        private void SetUpAudioListeners()
        {
            AudioListener[] audiolistenersInScene = FindObjectsOfType<AudioListener>();

            foreach (AudioListener audi in audiolistenersInScene)
            {
                if (audi.gameObject.name != "AudioListenerGameObject")
                {
                    if (printDebugMessages) print("destroying listener on " + audi.gameObject.name);
                    Destroy(audi);
                }
            }

            if (usePlayerAudioListener)
            {

                GameObject player = null;

                try
                {
                    player = GameObject.Find("Player");
                    if (printDebugMessages) print("Finding player...!");
                }
                catch
                {
                    if (printDebugMessages) print("Player not found!");
                }

                if (player != null)
                {
                    if (printDebugMessages) print("Player found!");
                    audioListenerGameObject.transform.SetParent(player.transform, false);

                }

            }

            audioListenerGameObject.transform.position = new Vector3(0, 0, -5);

        }

        /// <summary>
        /// Starts a SoundBase on a Transform.  
        /// </summary>
        /// <param name="soundBase">The SoundBase to play. </param>
        /// <param name="target">The Transform to play the SoundBase on. </param>
        /// <param name="makeUnique">If true will create a new AudioSource if there is one currently playing on the Target. </param>
        public void PlaySoundBaseOnTarget(SoundBase soundBase, Transform target, bool makeUnique)
        {
            
            if (!SoundShouldPlay(soundBase)) { return; }

            PlaySoundBaseOnAudioSource(soundBase, GetAudioSourceFromTarget(target, makeUnique));

        }

        /// <summary>
        /// Plays a OneShot on a Transform. OneShots cannot have pitch automation or loop.  
        /// </summary>
        /// <param name="sound">The BasicSound to play as a OneShot. </param>
        /// <param name="target">The Transform to play the BasicSound on. </param>
        public void PlayOneshotOnTarget(BasicSound sound, Transform target)
        {

            if (!SoundShouldPlay(sound)) { return; }

            PlayOneshot(sound, GetAudioSourceFromTarget(target, false));

        }

        /// <summary>
        /// Returns any AUdioSource found on the target Transform. If there are none or we want a unique sound we will create and return a new AudioSource.
        /// </summary>
        /// <param name="target">The Transform to get the AudioSource from. </param>
        /// <param name="makeUnique">If true will create a new AudioSource if there is one currently playing on the Target. </param>
        /// <returns> Always returns an AudioSource even if there is no AudioSource on the target Transform </returns>
        private AudioSource GetAudioSourceFromTarget(Transform target, bool makeUnique)
        {

            AudioSource targetAudioSource = target.GetComponent<AudioSource>();

            if (targetAudioSource == null || (makeUnique && targetAudioSource.isPlaying))
            {
                targetAudioSource = target.gameObject.AddComponent<AudioSource>();

                if (makeUnique)
                {
                    audioSourcesToDestroy.Add(targetAudioSource);
                }

            }

            return targetAudioSource;
        }

        /// <summary>
        /// Create an AudioSource GameObject at a specific position and play a sound. 
        /// </summary>
        /// <param name="soundBase">The SoundBase to start playing at a location.</param>
        /// <param name="vector">The location for the SoundBase to be played.</param>
        /// <param name="objCalledFrom">The name of the object creating the AudioSource GameObject.</param>
        public void PlaySoundBaseAtPos(SoundBase soundBase, Vector2 vector, string objCalledFrom)
        {

            if (!SoundShouldPlay(soundBase)) { return; }

            GameObject audioSourceGameObject = new GameObject();
            audioSourceGameObject.transform.name = $"Playing {soundBase.name} at {vector}. Created by {objCalledFrom}"; //can be deleted for final builds
            audioSourceGameObject.transform.position = vector;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            PlaySoundBaseOnAudioSource(soundBase, audioSource);

            soundsToDestroyList.Add(soundBase);
        }

        /// <summary>
        /// Plays a SoundBase on an AudioSource. 
        /// </summary>
        /// <param name="soundBase">The SoundBase to play on the specified AudioSource.</param>
        /// <param name="audioSource">The AudioSource that will play the SoundBase.</param>
        private void PlaySoundBaseOnAudioSource(SoundBase soundBase, AudioSource audioSource)
        {

            soundBase.audioSourceInUse = audioSource;

            switch (soundBase.GetSoundType())
            {
                case SoundType.BasicSound:
                {

                    BasicSound sound = (BasicSound)soundBase;
                    PlaySound(sound, audioSource); 

                    break;
                }
                    
                case SoundType.SoundContainer:
                {

                    SoundContainer container = (SoundContainer)soundBase;
                    container.shouldPlay = true;
                    StartCoroutine(PlaySoundContainer(container, audioSource));

                    break;
                }

                default:
                {
                    print("Default case in AudioManager.PlaySoundBaseOnAudioSource reached!\nSoundBases can only be of type 'BasicSound' or 'SoundContainer'");
                    break;
                }
            }
        }

        /// <summary>
        /// Plays a BasicSound on an AudioSource. 
        /// </summary>
        /// <param name="sound">The BasicSound to start playing at a location.</param>
        /// <param name="audioSource">The AudioSource that will play the BasicSound.</param>
        private void PlaySound(BasicSound sound, AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                GameObject go = audioSource.gameObject;
                audioSource = go.AddComponent<AudioSource>();

            }

            audioSource.clip = sound.audioClip;
            ApplySoundSettingsToAudioSource(sound, audioSource);

            if (sound.audioClip != null)
                audioSource.Play();
            else
                print($"Tried to play a sound ({sound.name}) on {audioSource.gameObject.name}.");

        }

        /// <summary>
        /// Plays a BasicSound on an AudioSource as a OneShot. 
        /// </summary>
        /// <param name="sound">The BasicSound to play as a OneShot.</param>
        /// <param name="audioSource">The AudioSource to play the OneShot on.</param>
        private void PlayOneshot(BasicSound sound,  AudioSource audioSource)
        {

            AudioSource newAudioSource = audioSource;

            float volume = sound.GetVolume();

            if (sound.audioClip != null)
                newAudioSource.PlayOneShot(sound.audioClip, volume);
            else
                print($"Tried to play a oneshot sound ({sound.name}) on {newAudioSource.gameObject.name}.");

        }

        /// <summary>
        /// Creates and initializes an AverageAudio GameObject.
        /// </summary>
        /// <param name="sound">The BasicSound to play on the AverageAudio GameObject.</param>
        /// <returns> Returns an AverageAudio with a BasicSound initialized. </returns>
        public AverageAudio CreateAndPlayAverageAudioSource(BasicSound sound)
        {
            if (!sound.IsValid()) return null;

            GameObject averageAudioGameObject = new GameObject();
            averageAudioGameObject.transform.name = $"AverageAudio ({sound.name}) GameObj";
            AudioSource audioSourceAdded = averageAudioGameObject.AddComponent<AudioSource>();

            AverageAudio averageAudio = averageAudioGameObject.AddComponent<AverageAudio>();
            averageAudio.audioSource = audioSourceAdded;
            averageAudio.sound = sound;
            PlaySoundBaseOnAudioSource(averageAudio.sound, audioSourceAdded);
            soundsToDestroyList.Add(sound);

            return averageAudio;

        }

        /// <summary>
        /// Loops through the sounds in the Container and plays them back based on the SoundContainerType of the SoundContainer.
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to be used. </param>
        /// <param name="audioSource">The AudioSource to use for playback. </param>
        private IEnumerator PlaySoundContainer(SoundContainer soundContainer, AudioSource audioSource)
        {

            int soundsLength = soundContainer.clipsInContainer.Length;

            if (soundsLength < 1)
            {
                print($"Tried to play a sound container ({soundContainer.name}) on {audioSource.gameObject.name}.");
                yield break;
            }

            soundContainer.isPlaying = true;
            activeSoundContainers.Add(soundContainer);

            switch (soundContainer.containerType)
            {

                //Plays through each AudioClip in the container from first -> last
                case SoundContainerType.Sequential:
                    foreach (var audioClip in soundContainer.clipsInContainer)
                    {

                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, audioClip);
                        audioSource.Play();
                        float awaitTime = audioClip.length;
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay || !soundContainer.IsValid())
                            break;
                    }

                    break;

                //Plays through each AudioClip in the container randomly, but never playing each sound more than once per loop
                case SoundContainerType.RandomSequential:

                    List<AudioClip> clips = soundContainer.clipsInContainer.ToList();

                    for (int i = 0; i < soundsLength; i++)
                    {

                        int randomInt = random.Next(clips.Count);
                        AudioClip clipToPlay = clips[randomInt];
                        float awaitTime = clipToPlay.length;
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, clipToPlay);

                        audioSource.Play();
                        clips.Remove(clipToPlay);
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay || !soundContainer.IsValid())
                            break;
                    }

                    break;

                //Plays through each AudioClip in the container randomly, not caring if a sound plays more than once per loop
                case SoundContainerType.RandomRandom:

                    for (int i = 0; i < soundsLength; i++)
                    {
                        int randomInt = random.Next(soundsLength);
                        AudioClip clipToPlay = soundContainer.clipsInContainer[randomInt];
                        float awaitTime = clipToPlay.length;
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, clipToPlay);
                        audioSource.Play();
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay || !soundContainer.IsValid())
                            break;
                    }

                    break;

                //Plays only one random AudioClip in the SoundContainer
                case SoundContainerType.RandomOneshot:

                    if (!soundContainer.IsValid())
                    {
                        print($"No clips found in {soundContainer.name} trying to play on {audioSource.gameObject.name}");
                        break;
                    }

                    soundContainer.loopContainer = false;

                    AudioClip oneshotToPlay = soundContainer.clipsInContainer[random.Next(soundsLength)];

                    if (!audioSource.isPlaying)
                    {
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, oneshotToPlay);
                    }

                    audioSource.PlayOneShot(oneshotToPlay);

                    float clipLength = oneshotToPlay.length;
                    StartCoroutine(HandleRandomOneShotPlayback(soundContainer, clipLength));

                    break;

            }

            if (soundContainer.loopContainer)
                StartCoroutine(PlaySoundContainer(soundContainer, audioSource));
            else if (soundContainer.containerType != SoundContainerType.RandomOneshot)
            {
                soundContainer.isPlaying = false;
            }
        }

        /// <summary>
        /// Stops a SoundContainer after a specified time. 
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to control.</param>
        /// <param name="clipLength">The length of time to wait before stopping the SoundContainer.</param>
        private IEnumerator HandleRandomOneShotPlayback(SoundContainer soundContainer, float clipLength)
        {
            yield return new WaitForSeconds(clipLength);
            soundContainer.isPlaying = false;
        }

        /// <summary>
        /// Applies the settings on a BasicSound to an AudioSource. Random values are assigned in this method.
        /// </summary>
        /// <param name="sound">The BasicSound to get the settings from.</param>
        /// <param name="audioSource">The AudioSource to apply the settings onto.</param>
        public void ApplySoundSettingsToAudioSource(BasicSound sound, AudioSource audioSource)
        {

            if (!SoundShouldPlay(sound)) { return; }

            audioSource.clip = sound.audioClip;
            audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;
            sound.audioSourceInUse = audioSource;

            if (sound.useDefaultSettings) //default settings set on AudioManager Component
            {

                audioSource.priority = _defaultSoundSettings.priority;
                audioSource.loop = _defaultSoundSettings.loop;
                audioSource.pitch = _defaultSoundSettings.pitch;
                audioSource.volume = _defaultSoundSettings.volume;
                audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;
                audioSource.spread = _defaultSoundSettings.spread;

            }
            else //use settings found on the Sound.
            {
                audioSource.priority = sound.soundSettings.priority;
                audioSource.loop = sound.soundSettings.loop;
                audioSource.volume = sound.GetVolume();
                audioSource.pitch = sound.GetPitch();
                audioSource.spatialBlend = sound.soundSettings.spatialBlend;
                audioSource.spread = sound.soundSettings.spread;
            }
        }

        /// <summary>
        /// Applies the settings on a SoundContainer to an AudioSource. Random values are assigned in this method.
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to get the settings from.</param>
        /// <param name="audioSource">The AudioSource to apply the settings onto.</param>
        /// <param name="clip">The AudioClip to assign to the AudioSource.</param>
        public void ApplySoundSettingsToAudioSource(SoundContainer soundContainer, AudioSource audioSource, AudioClip clip)
        {

            if (!SoundShouldPlay(soundContainer)) { return; }

            //audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = soundContainer.outputAudioMixerGroup;
            soundContainer.audioSourceInUse = audioSource;

            if (soundContainer.useDefaultSettings) //default settings set on AudioManager Component
            {

                audioSource.priority = _defaultSoundSettings.priority;
                audioSource.pitch = _defaultSoundSettings.pitch;
                audioSource.volume = _defaultSoundSettings.volume;
                audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;
                audioSource.spread = _defaultSoundSettings.spread;

            }
            else //use settings found on the SoundContainer.
            {
                audioSource.priority = soundContainer.soundSettings.priority;
                audioSource.volume = soundContainer.GetVolume();
                audioSource.pitch = soundContainer.GetPitch();
                audioSource.spatialBlend = soundContainer.soundSettings.spatialBlend;
                audioSource.spread = soundContainer.soundSettings.spread;
            }
        }

        /// <summary>
        /// Stops all active sound containers.
        /// </summary>
        public void StopAllContainers()
        {
            StopAllCoroutines();
            foreach (SoundBase sb in activeSoundContainers)
            {
                sb.Stop();
            }
        }

        /// <summary>
        /// Checking for expired audio and destroying their objects on the FixedUpdate. 
        /// </summary>
        private void FixedUpdate()
        {
            DestroyExpiredAudio();
        }

        /// <summary>
        /// Destroy audio in the scene after it has finished playing. 
        /// </summary>
        private void DestroyExpiredAudio()
        {
         
            foreach (SoundBase sb in soundsToDestroyList)
            {
                if (sb == null)
                {
                    soundsToDestroyList.Remove(sb);
                    //return;
                }

                if (!sb.IsPlaying())
                {
                    soundsToDestroyList.Remove(sb);
                    sb.Stop();
                    //print("destroying " + sb.name);
                    sb.DestroyObject();
                    return;
                }
            }

            foreach (AudioSource audioSource in audioSourcesToDestroy)
            {
                if (audioSource == null)
                {
                    audioSourcesToDestroy.Remove(audioSource);
                    return;
                }

                if (!audioSource.isPlaying)
                {
                    audioSourcesToDestroy.Remove(audioSource);
                    audioSource.Stop();
                    Destroy(audioSource);
                    return;
                }
            }
        }

        /// <summary>
        /// Starts the Coroutine that fades the volume of an AudioSource out then destroys an Object.
        /// </summary>
        /// <param name="audioSourceToFade">The AudioSource to fade the volume of. </param>
        /// <param name="startValue">The initial volume. </param>
        /// <param name="duration">How long the fade lasts. </param>
        /// <param name="destroyObjectOrAudioSource">If true will destroy the GameObject attached to the AudioSource, otherwise will destroy the AudioSource if there is more than one on the GameObject. </param>
        public void FadeToDestroy(AudioSource audioSourceToFade, float startValue, float duration, bool destroyObjectOrAudioSource)
        {
            StartCoroutine(FadeToDestroyCoroutine(audioSourceToFade, startValue, duration, destroyObjectOrAudioSource));
        }

        /// <summary>
        /// Fades the volume of an AudioSource out then destroys an Object.
        /// </summary>
        /// <param name="audioSourceToFade">The AudioSource to fade the volume of. </param>
        /// <param name="startValue">The initial volume. </param>
        /// <param name="duration">How long the fade lasts. </param>
        /// <param name="destroyObjectOrAudioSource">If true will destroy the GameObject attached to the AudioSource, otherwise will destroy the AudioSource if there is more than one on the GameObject. </param>
        public IEnumerator FadeToDestroyCoroutine(AudioSource audioSourceToFade, float startValue, float duration, bool destroyObjectOrAudioSource)
        {

            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                audioSourceToFade.volume = Mathf.Lerp(startValue, 0, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            if (destroyObjectOrAudioSource)
            {
                Destroy(audioSourceToFade.gameObject);

            }
            else
            {

                AudioSource[] sources = audioSourceToFade.gameObject.GetComponents<AudioSource>();
                if (sources.Length <= 1)
                {
                    audioSourceToFade.Stop();
                    audioSourceToFade.clip = null;
                }
                else
                {
                    Destroy(audioSourceToFade);
                }

            } 
        }

        /// <summary>
        /// Applies SoundSettings of one SoundBase to another SoundBase.
        /// </summary>
        /// <param name="soundToCopySettings">The SoundBase to get the SoundSettings from. </param>
        /// <param name="soundToApplySettings">The SoundBase to apply new SoundSettings to.</param>
        public void ApplySoundSettingsToSound(SoundBase soundToCopySettings, SoundBase soundToApplySettings)
        {

            if (!SoundShouldPlay(soundToCopySettings) || !SoundShouldPlay(soundToApplySettings)) { return; }

            soundToApplySettings.outputAudioMixerGroup = soundToCopySettings.outputAudioMixerGroup;
            soundToApplySettings.soundSettings = soundToCopySettings.soundSettings;
        }

        /// <summary>
        /// Checks whether or not a SoundBase should play.
        /// </summary>
        /// <param name="sound">The SoundBase to check whether or not it should play. </param>
        /// <returns> Returns true if a SoundBase should play, false if it shouldn't. </returns>
        private bool SoundShouldPlay(SoundBase sound)
        {
            
            if (!SceneManager.GetActiveScene().isLoaded) 
            {
                if (printDebugMessages) print($"scene is not loaded! could not play {sound.name}!");  
                return false; 
            }
            else if (!sound.IsValid()) 
            {
                if (printDebugMessages) print($"sound is not valid! could not play {sound.name}!"); 
                return false; 
            }
            else if (sound.SoundInCooldown(Time.time)) 
            {
                if (printDebugMessages) print($"sound in cooldown! could not play {sound.name}!"); 
                return false;
            }
            else { return true; }

        }

    }

}

