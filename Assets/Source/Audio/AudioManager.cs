using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;
using System.Collections;
using System.Linq;

namespace Cardificer
{

    /// <summary>
    /// An Audio Manager meant to coordinate all audio for the game, ideally and eventually. Play sounds at points, contain the music system, etc. 
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        // Singleton Pattern
        public static AudioManager instance;

        //[Tooltip("Sound Arrays for later Random Container Use")]
        //public Sound[] sounds;

        [Tooltip("List of all currently playing audio sources")]
        [SerializeField] public List<SoundBase> soundsToKillList = new List<SoundBase>();

        [Tooltip("List of Projectiles to pull the average audio position from")]
        private List<AverageAudio> averageAudioList = new List<AverageAudio>();

        public SoundSettings _defaultSoundSettings;
        private System.Random random = new System.Random();
        private List<SoundContainer> activeSoundContainers = new List<SoundContainer>();

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
                StopAllContainers();
        }

        public void StopAllContainers()
        {
            StopAllCoroutines();
            foreach (SoundBase sb in activeSoundContainers)
            {
                sb.Stop();
            }
        }

        /// <summary>
        /// Play attack card SFX when those cards are played that originate from the player.  
        /// </summary>
        /// <param name="audioClip">The AudioClip of the Attack Card to be played. </param>
        public void PlayAudioAtActor(Sound _sound, IActor _actor)
        {
            var actorAudioSource = _actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                ApplySoundSettingsToAudioSource(_sound, actorAudioSource);
                actorAudioSource.Play();
            }

        }

        public void PlayAudioAtActor(SoundContainer _soundContainer, IActor _actor)
        {
            var actorAudioSource = _actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                _soundContainer.shouldPlay = true;
                StartCoroutine(PlaySoundContainer(_soundContainer, actorAudioSource));
            }

        }

        private IEnumerator PlaySoundContainer(SoundContainer soundContainer, AudioSource audioSource)
        {

            int soundsLength = soundContainer.sounds.Length;
            soundContainer.isPlaying = true;
            activeSoundContainers.Add(soundContainer);

            switch (soundContainer.containerType)
            {
                case SoundContainerType.Sequential:
                    foreach (var audioClip in soundContainer.sounds)
                    {
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, audioClip);
                        audioSource.Play();
                        float awaitTime = audioClip.length;
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay)
                            break;

                    }

                    break;

                case SoundContainerType.RandomSequential:

                    List<AudioClip> clips = soundContainer.sounds.ToList();

                    for (int i = 0; i < soundsLength; i++)
                    {

                        int randomInt = random.Next(clips.Count);
                        AudioClip clipToPlay = clips[randomInt];
                        float awaitTime = clipToPlay.length;
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, clipToPlay);
                        Debug.Log($"Now playing {clipToPlay.name}");
                        audioSource.Play();
                        clips.Remove(clipToPlay);
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay)
                            break;
                    }

                    break;

                case SoundContainerType.RandomRandom:

                    for (int i = 0; i < soundsLength; i++)
                    {
                        int randomInt = random.Next(soundsLength);
                        AudioClip clipToPlay = soundContainer.sounds[randomInt];
                        float awaitTime = clipToPlay.length;
                        ApplySoundSettingsToAudioSource(soundContainer, audioSource, clipToPlay);
                        audioSource.Play();
                        yield return new WaitForSeconds(awaitTime);

                        if (!soundContainer.shouldPlay)
                            break;
                    }

                    break;

                case SoundContainerType.RandomOneshot:

                    soundContainer.loopContainer = false;

                    AudioClip oneshotToPlay = soundContainer.sounds[random.Next(soundsLength)];
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

            //AudioClip RandomAudioClipFromArray(AudioClip[] clipsToChooseFrom, out AudioClip[] newClipList)
            //{
            //    AudioClip[] listToSendBack = clipsToChooseFrom;
            //}
        }



        /// <summary>
        /// Checking for expired audio and destroying their objects on the Fixed Update. 
        /// </summary>
        private void FixedUpdate()
        {
            DestroyExpiredAudio();
        }

        /// <summary>
        /// Create an AudioSource GameObject at a specific position and play the associated clip. 
        /// </summary>
        /// <param name="audioClip">The audio clip to be played at a location.</param>
        /// <param name="vector">The location for the audio clip to be played.</param>
        public void PlayAudioAtPos(Sound sound, Vector2 vector)
        {
            if (!SceneManager.GetActiveScene().isLoaded) { return; }
            GameObject audioSourceGameObject = new GameObject();
            audioSourceGameObject.transform.name = $"Play{sound.name}At{vector}Obj";
            audioSourceGameObject.transform.position = vector;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            ApplySoundSettingsToAudioSource(sound, audioSource);
            sound.audioSourceInUse = audioSource;
            audioSource.Play();

            soundsToKillList.Add(sound);
        }

        public void PlayAudioAtPos(SoundContainer soundContainer, Vector2 vector)
        {
            if (!SceneManager.GetActiveScene().isLoaded) { return; }
            GameObject audioSourceGameObject = new GameObject();
            audioSourceGameObject.transform.name = $"Play{soundContainer.name}At{vector}Obj";
            audioSourceGameObject.transform.position = vector;

            AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
            soundContainer.audioSourceInUse = audioSource;
            StartCoroutine(PlaySoundContainer(soundContainer, audioSource));

            soundsToKillList.Add(soundContainer);
        }

        /// <summary>
        /// Get the average audio location of multiple projectiles and create a gameobject with an audio source at the average position to play. 
        /// </summary>
        /// <param name="projectiles">The list of projectiles to find the average position of. </param>
        /// <param name="audioClip">The audioclip to play at the location. </param>
        /// <param name="averageOrFirst">Determines whether to get the average location of all projectiles or just use one projectile</param>
        public void PlayAverageAudio(List<Projectile> projectiles, Sound sound, bool averageOrFirst)
        {

            if (averageOrFirst == true)
            {
                AverageAudio averageAudioGameObject = new GameObject().AddComponent<AverageAudio>();
                averageAudioGameObject.transform.name = $"AverageAudio({sound.name})GameObj";

                AverageAudio averageAudioComponent = averageAudioGameObject.GetComponent<AverageAudio>();
                averageAudioComponent.SetProjectilesAndSound(projectiles, sound);
                averageAudioGameObject.transform.position = averageAudioGameObject.TryGetAveragePos();
                averageAudioComponent.PlayAverageAudio();

                averageAudioList.Add((averageAudioGameObject));

            }

            else
            {
                
                if (projectiles.Count == 0 || projectiles[0] == null)
                    return; 

                var audioSource = projectiles[0].gameObject.AddComponent<AudioSource>();
                ApplySoundSettingsToAudioSource(sound, audioSource);
                audioSource.Play();
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
        /// <summary>
        /// Destroy audio in the scene after it has finished playing. 
        /// </summary>
        private void DestroyExpiredAudio()
        {

            foreach (SoundBase sb in soundsToKillList)
            {
                print($"{sb.name} IsPlaying() = {sb.IsPlaying()}");
            }

            foreach (SoundBase sb in soundsToKillList)
            {
                if (!sb.IsPlaying())
                {
                    soundsToKillList.Remove(sb);
                    sb.Stop();
                    sb.DestroyObject();
                    return;
                }
            }
        }

        public void ApplySoundSettingsToAudioSource(Sound _sound, AudioSource _audioSource)
        {
            _audioSource.clip = _sound.audioClip;
            _audioSource.outputAudioMixerGroup = _sound.outputAudioMixerGroup;

            if (_sound.useDefaultSettings)
            {

                _audioSource.priority = _defaultSoundSettings.priority;
                _audioSource.loop = _defaultSoundSettings.loop;
                _audioSource.pitch = _defaultSoundSettings.pitch;
                _audioSource.volume = _defaultSoundSettings.volume;
                _audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;

            }
            else
            {
                _audioSource.priority = _sound.soundSettings.priority;
                _audioSource.loop = _sound.soundSettings.loop;

                if (_sound.soundSettings.randomizePitch)
                {
                    _audioSource.pitch = Random.Range(_sound.soundSettings.pitchRandomRange.x, _sound.soundSettings.pitchRandomRange.y);
                }
                else
                {
                    _audioSource.pitch = _sound.soundSettings.pitch;
                }

                if (_sound.soundSettings.randomizeVolume)
                {
                    _audioSource.volume = Random.Range(_sound.soundSettings.volumeRandomRange.x, _sound.soundSettings.volumeRandomRange.y);
                }
                else
                {
                    _audioSource.volume = _sound.soundSettings.volume;
                }

                _audioSource.spatialBlend = _sound.soundSettings.spatialBlend;
            }
        }

        public void ApplySoundSettingsToAudioSource(SoundContainer _soundContainer, AudioSource _audioSource, AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.outputAudioMixerGroup = _soundContainer.outputAudioMixerGroup;

            if (_soundContainer.useDefaultSettings)
            {

                _audioSource.priority = _defaultSoundSettings.priority;
                _audioSource.loop = _defaultSoundSettings.loop;
                _audioSource.pitch = _defaultSoundSettings.pitch;
                _audioSource.volume = _defaultSoundSettings.volume;
                _audioSource.spatialBlend = _defaultSoundSettings.spatialBlend;

            }
            else
            {
                _audioSource.priority = _soundContainer.soundSettings.priority;
                //_audioSource.loop = _soundContainer.soundSettings.loop;

                if (_soundContainer.soundSettings.randomizePitch)
                {
                    _audioSource.pitch = Random.Range(_soundContainer.soundSettings.pitchRandomRange.x, _soundContainer.soundSettings.pitchRandomRange.y);
                }
                else
                {
                    _audioSource.pitch = _soundContainer.soundSettings.pitch;
                }

                if (_soundContainer.soundSettings.randomizeVolume)
                {
                    _audioSource.volume = Random.Range(_soundContainer.soundSettings.volumeRandomRange.x, _soundContainer.soundSettings.volumeRandomRange.y);
                }
                else
                {
                    _audioSource.volume = _soundContainer.soundSettings.volume;
                }

                _audioSource.spatialBlend = _soundContainer.soundSettings.spatialBlend;
            }
        }

    }

    [System.Serializable]
    public abstract class SoundBase
    {
        public abstract string name { get; }
        public AudioMixerGroup outputAudioMixerGroup;
        public bool useDefaultSettings;
        public SoundSettings soundSettings;
        public AudioSource audioSourceInUse;
        public abstract bool IsPlaying();
        public abstract void Stop();
        public abstract void DestroyObject();
    }

    /// <summary>
    /// This class is for later implementation of random containers, random pitch and volume and overall a more fleshed out audio manager.
    /// </summary>
    [System.Serializable]
    public class Sound : SoundBase
    {

        [Header("Sound Settings")]
        public AudioClip audioClip;
        public override string name { get { return audioClip.name; } }

        public override void Stop()
        {
            audioSourceInUse.Stop();
        }

        public override bool IsPlaying()
        {
            if (audioSourceInUse != null)
                return audioSourceInUse.isPlaying;
            else
                return false;
        }

        public override void DestroyObject()
        {
            if (audioSourceInUse != null)
                Object.Destroy(audioSourceInUse.gameObject);
        }

    }

    [System.Serializable]
    public class SoundSettings
    {
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

        public override bool IsPlaying()
        {
            return isPlaying;
        }

        public override void Stop()
        {
            isPlaying = false;
            shouldPlay = false;
            if (audioSourceInUse != null)
                audioSourceInUse.Stop();
        }

        public override void DestroyObject()
        {
            if (audioSourceInUse != null)
                Object.Destroy(audioSourceInUse.gameObject);
        }


    }

    public enum SoundContainerType
    {
        Sequential,
        RandomSequential,
        RandomRandom,
        RandomOneshot,
        //RandomBurst,
    }

}