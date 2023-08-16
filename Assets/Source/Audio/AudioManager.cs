using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;
using System.Collections;
using System.Linq;

namespace Cardificer
{

    /// <summary>
    /// Contains all the logic to playback audio. Currently plays back Sounds and SoundContainers, and eventually music.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        //Singleton Pattern
        public static AudioManager instance;

        //Lists of objects to be destroyed or affected
        private List<SoundBase> soundsToDestroyList = new List<SoundBase>();
        private List<AverageAudio> averageAudioList = new List<AverageAudio>();
        private List<SoundContainer> activeSoundContainers = new List<SoundContainer>();

        [Tooltip("Default SoundSettings to be applied when a SoundBase has 'Use Default Settings' set to true.")]
        public SoundSettings _defaultSoundSettings;

        //Serialized Random for use in random SoundContainer playback
        private System.Random random = new System.Random();

        /// <summary>
        /// Implementing the singleton pattern. 
        /// </summary>
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("There's more than one AudioManager!" + transform + " - " + instance);
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        /// <summary>
        /// For testing methods.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                StopAllContainers();
        }

        /// <summary>
        /// Play an AudioClip on an AudioSource attatched to an IActor. This method uses the settings of the AudioSource already on the IActor, and does not apply any other SoundSettings.
        /// </summary>
        /// <param name="audioClip">The AudioClip to be played. </param>
        /// <param name="actor">The IActor to get the AudioSource from. </param>
        public void PlaySoundOnActor(AudioClip audioClip, IActor actor)
        {
            Debug.LogWarning("You are using a depreciated method of playing audio. Please use Sounds instead of AudioClips to play audio.");

            var actorAudioSource = actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                actorAudioSource.clip = audioClip;
                actorAudioSource.Play();
            }

        }

        /// <summary>
        /// Play a Sound on an AudioSource attatched to an IActor. 
        /// </summary>
        /// <param name="sound">The Sound to be played. </param>
        /// <param name="actor">The IActor to get the AudioSource from. </param>
        public void PlaySoundOnActor(Sound sound, IActor actor)
        {
            var actorAudioSource = actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                ApplySoundSettingsToAudioSource(sound, actorAudioSource);
                actorAudioSource.Play();
            }

        }

        /// <summary>
        /// Starts a SoundContainer on an AudioSource attatched to an IActor.  
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to start. </param>
        /// <param name="actor">The IActor to get the AudioSource from. </param>
        public void PlaySoundOnActor(SoundContainer soundContainer, IActor actor)
        {
            var actorAudioSource = actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                soundContainer.shouldPlay = true;
                StartCoroutine(PlaySoundContainer(soundContainer, actorAudioSource));
            }

        }

        /// <summary>
        /// Create an AudioSource GameObject at a specific position and play the associated Sound. 
        /// </summary>
        /// <param name="sound">The Sound to be played at a location.</param>
        /// <param name="vector">The location for the Sound to be played.</param>
        public void PlaySoundAtPos(Sound sound, Vector2 vector)
        {
            if (!SceneManager.GetActiveScene().isLoaded) { return; }
            GameObject audioSourceGameObject = new GameObject();
            audioSourceGameObject.transform.name = $"Play{sound.name}At{vector}Obj";
            audioSourceGameObject.transform.position = vector;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            ApplySoundSettingsToAudioSource(sound, audioSource);
            sound.audioSourceInUse = audioSource;
            audioSource.Play();

            soundsToDestroyList.Add(sound);
        }

        /// <summary>
        /// Create an AudioSource GameObject at a specific position and start the associated SoundContainer. 
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to be started at a location.</param>
        /// <param name="vector">The location for the SoundContainer to be played.</param>
        public void PlaySoundAtPos(SoundContainer soundContainer, Vector2 vector)
        {
            if (!SceneManager.GetActiveScene().isLoaded) { return; }
            GameObject audioSourceGameObject = new GameObject();
            audioSourceGameObject.transform.name = $"Play{soundContainer.name}At{vector}Obj";
            audioSourceGameObject.transform.position = vector;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            soundContainer.audioSourceInUse = audioSource;
            StartCoroutine(PlaySoundContainer(soundContainer, audioSource));

            soundsToDestroyList.Add(soundContainer);
        }

        /// <summary>
        /// Get the average audio location of multiple projectiles and create a gameobject with an audio source at the average position to play. 
        /// </summary>
        /// <param name="projectiles">The list of projectiles to find the average position of. </param>
        /// <param name="sound">The audioclip to play at the location. </param>
        /// <param name="averageOrFirst">Determines whether to get the average location of all projectiles or just use one projectile</param>
        public void PlaySoundAtAveragePos(List<Projectile> projectiles, Sound sound, bool averageOrFirst)
        {

            if (averageOrFirst == true) //play at average position
            {
                //Create a GameObject and attach an AverageAudio component to it
                AverageAudio averageAudioGameObject = new GameObject().AddComponent<AverageAudio>();
                averageAudioGameObject.transform.name = $"AverageAudio({sound.name})GameObj";

                //Initialize the AverageAudio component and play the sound. The AverageAudio component keeps the AudioSource's GameObject at the average location of the projectiles
                AverageAudio averageAudioComponent = averageAudioGameObject.GetComponent<AverageAudio>();
                averageAudioComponent.SetProjectilesAndSound(projectiles, sound);
                averageAudioGameObject.transform.position = averageAudioGameObject.TryGetAveragePos();
                averageAudioComponent.PlayAverageAudio();

                averageAudioList.Add(averageAudioGameObject);

            }

            else //play at first projectile
            {
                
                if (projectiles.Count == 0 || projectiles[0] == null)
                    return; 

                var audioSource = projectiles[0].gameObject.AddComponent<AudioSource>();
                ApplySoundSettingsToAudioSource(sound, audioSource);
                audioSource.Play();
            }
        }

        /// <summary>
        /// Loops through the sounds in the Container and plays them back based on the SoundContainerType of the SoundContainer.
        /// </summary>
        /// <param name="soundContainer">The SoundContainer to be used. </param>
        /// <param name="audioSource">The AudioSource to use for playback. </param>
        private IEnumerator PlaySoundContainer(SoundContainer soundContainer, AudioSource audioSource)
        {

            int soundsLength = soundContainer.clipsInContainer.Length;
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

                        if (!soundContainer.shouldPlay)
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

                        if (!soundContainer.shouldPlay)
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

                        if (!soundContainer.shouldPlay)
                            break;
                    }

                    break;

                //Plays only one random AudioClip in the SoundContainer
                case SoundContainerType.RandomOneshot:

                    soundContainer.loopContainer = false;

                    AudioClip oneshotToPlay = soundContainer.clipsInContainer[random.Next(soundsLength)];
                    ApplySoundSettingsToAudioSource(soundContainer, audioSource, oneshotToPlay);
                    audioSource.PlayOneShot(oneshotToPlay);

                    break;

            }

            if (soundContainer.loopContainer)
                StartCoroutine(PlaySoundContainer(soundContainer, audioSource));
            else
            {
                soundContainer.isPlaying = false;
            }
        }

        /// <summary>
        /// Applies the settings on a Sound to an AudioSource. Random values are assigned in this method.
        /// </summary>
        /// <param name="sound">The Sound to get the settings from.</param>
        /// <param name="audioSource">The AudioSource to apply the settings onto.</param>
        public void ApplySoundSettingsToAudioSource(Sound sound, AudioSource audioSource)
        {
            audioSource.clip = sound.audioClip;
            audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;

            if (sound.useDefaultSettings) //default settings set on AudioManager Component
            {

                audioSource.priority = _defaultSoundSettings.priority;
                audioSource.loop = _defaultSoundSettings.loop;
                audioSource.pitch = _defaultSoundSettings.pitch;
                audioSource.volume = _defaultSoundSettings.volume;
                audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;

            }
            else //use settings found on the Sound. Apply random values if wanted
            {
                audioSource.priority = sound.soundSettings.priority;
                audioSource.loop = sound.soundSettings.loop;

                if (sound.soundSettings.randomizePitch)
                {
                    audioSource.pitch = Random.Range(sound.soundSettings.pitchRandomRange.x, sound.soundSettings.pitchRandomRange.y);
                }
                else
                {
                    audioSource.pitch = sound.soundSettings.pitch;
                }

                if (sound.soundSettings.randomizeVolume)
                {
                    audioSource.volume = Random.Range(sound.soundSettings.volumeRandomRange.x, sound.soundSettings.volumeRandomRange.y);
                }
                else
                {
                    audioSource.volume = sound.soundSettings.volume;
                }

                audioSource.spatialBlend = sound.soundSettings.spatialBlend;
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
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = soundContainer.outputAudioMixerGroup;

            if (soundContainer.useDefaultSettings) //default settings set on AudioManager Component
            {

                audioSource.priority = _defaultSoundSettings.priority;
                audioSource.loop = _defaultSoundSettings.loop;
                audioSource.pitch = _defaultSoundSettings.pitch;
                audioSource.volume = _defaultSoundSettings.volume;
                audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;

            }
            else //use settings found on the SoundContainer. Apply random values if wanted
            {
                audioSource.priority = soundContainer.soundSettings.priority;

                if (soundContainer.soundSettings.randomizePitch)
                {
                    audioSource.pitch = Random.Range(soundContainer.soundSettings.pitchRandomRange.x, soundContainer.soundSettings.pitchRandomRange.y);
                }
                else
                {
                    audioSource.pitch = soundContainer.soundSettings.pitch;
                }

                if (soundContainer.soundSettings.randomizeVolume)
                {
                    audioSource.volume = Random.Range(soundContainer.soundSettings.volumeRandomRange.x, soundContainer.soundSettings.volumeRandomRange.y);
                }
                else
                {
                    audioSource.volume = soundContainer.soundSettings.volume;
                }

                audioSource.spatialBlend = soundContainer.soundSettings.spatialBlend;
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
                if (!sb.IsPlaying())
                {
                    soundsToDestroyList.Remove(sb);
                    sb.Stop();
                    sb.DestroyObject();
                    return;
                }
            }
        }

        /// <summary>
        /// Kill the average audio object after it is finished playing
        /// </summary>
        /// <param name="averageAudio">What average audio object to kill</param>
        public static void KillAverageAudio(AverageAudio averageAudio)
        {

            instance.averageAudioList.Remove(averageAudio);
            Destroy(averageAudio.gameObject);

        }

    }


}