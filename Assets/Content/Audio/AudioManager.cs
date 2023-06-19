using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Cardificer
{



    /// <summary>
    /// An Audio Manager meant to coordinate all audio for the game, ideally and eventually. Play sounds at points, contain the music system, etc. 
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        //Singleton Pattern
        public static AudioManager instance;

        [Tooltip("Sound Arrays for later Random Container Use")]
        public Sound[] sounds;

        [Tooltip("List of all currently playing audio sources")]
        private List<AudioSource> audioSourcesList = new List<AudioSource>();

        [Tooltip("List of Projectiles to pull the average audio position from")]
        private List<AverageAudio> averageAudioList = new List<AverageAudio>();




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

            // foreach (Sound s in sounds)
            // { 
            //     s.source = gameObject.AddComponent<AudioSource>();
            //     s.source.clip = s.clip;
            //
            //     s.source.volume = s.volume;
            //     s.source.pitch = s.pitch;
            //     s.source.loop = s.loop;
            //         
            // }
        }



        /// <summary>
        /// Play attack card SFX when those cards are played that originate from the player.  
        /// </summary>
        /// <param name="audioClip">The AudioClip of the Attack Card to be played. </param>
        public void PlayAudioAtActor(AudioClip audioClip, IActor actor)
        {
            var actorAudioSource = actor.GetAudioSource();

            if (actorAudioSource != null)
            {
                actorAudioSource.clip = audioClip;
                actorAudioSource.loop = false;
                actorAudioSource.spatialBlend = .5f;
                actorAudioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
                actorAudioSource.Play();
            }

        }

        private void FixedUpdate()
        {
            DestroyExpiredAudio();
        }

        public void PlayAudioAtPos(AudioClip audioClip, Vector2 vector)
        {


            var audioSource = new GameObject().AddComponent<AudioSource>();

            audioSource.transform.position = vector;


            audioSource.clip = audioClip;
            audioSource.loop = false;
            audioSource.spatialBlend = .5f;
            audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
            audioSource.Play();

            audioSourcesList.Add(audioSource);
        }

        public void GetAverageAudioSource(List<Projectile> projectiles, AudioClip audioClip, bool averageOrFirst)
        {

            if (averageOrFirst == true)
            {
                var averageAudioGameObject = new GameObject().AddComponent<AverageAudio>();
                var averageAudioComponent = averageAudioGameObject.GetComponent<AverageAudio>();
                averageAudioComponent.SetProjectiles(projectiles);
                averageAudioComponent.SetAudioClip(audioClip);

                averageAudioList.Add((averageAudioGameObject));

                averageAudioGameObject.transform.position = averageAudioGameObject.TryGetAveragePos();
            }

            else
            {
                if (projectiles.Count == 0)
                {
                    return;
                }

                var audioSource = projectiles[0].gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.loop = true;
                //audioSource.playOnAwake = true; 
                audioSource.spatialBlend = .5f;
                audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
                audioSource.Play();
            }
        }

        public static void KillAverageAudio(AverageAudio averageAudio)
        {

            AudioManager.instance.averageAudioList.Remove(averageAudio);
            Destroy(averageAudio.gameObject);

        }

        private void DestroyExpiredAudio()
        {
            foreach (var audioSource in audioSourcesList)
            {
                if (!audioSource.isPlaying)
                {
                    audioSourcesList.Remove(audioSource);
                    Destroy(audioSource.gameObject);
                    return;
                }
            }
        }
    }




    /// <summary>
    /// This class is for later implementation of random containers, random pitch and volume and overall a more fleshed out audio manager.
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        public AudioClip clip;
        public string name;


        [Range(0, 1f)] public float volume;
        [Range(.1f, 3f)] public float pitch;

        public bool loop;


        [HideInInspector] public AudioSource source;
    }
}