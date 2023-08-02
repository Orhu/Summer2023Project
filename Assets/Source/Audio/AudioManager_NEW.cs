using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{

    public class AudioManager_NEW : MonoBehaviour
    {

        public static AudioManager_NEW instance;

        public int maxAudioSources;
        private AudioSource[] _audioSources;

        public Sound testSound;

        private void Awake()
        {
            #region assign instance
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            #endregion

            List<AudioSource> audioSources = new List<AudioSource>();

            for (int i = 0; i < maxAudioSources; i++)
            {
                AudioSource _as = gameObject.AddComponent<AudioSource>();
                audioSources.Add(_as);
            }

            _audioSources = audioSources.ToArray();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Play(testSound);
        }

        public void Play(Sound _sound)
        {
            foreach (AudioSource _as in _audioSources)
            {
                if (!_as.isPlaying)
                {
                    _sound.Initialize(_as);
                    break;
                }
            }
            _sound.Play();
        }

    }


}
