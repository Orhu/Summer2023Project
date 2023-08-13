//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//namespace Cardificer
//{

//    public class AudioManager_NEW : MonoBehaviour
//    {

//        public static AudioManager_NEW instance;

//        public int maxAudioSources;
//        private AudioSource[] _audioSources;
//        private Dictionary<string, AudioSource> _audioSourcesDict;

//        public Sound testSound;

//        private void Awake()
//        {
//            #region assign instance
//            if (instance == null)
//                instance = this;
//            else
//                Destroy(gameObject);
//            #endregion

//            List<AudioSource> audioSources = new List<AudioSource>();

//            for (int i = 0; i < maxAudioSources; i++)
//            {
//                AudioSource _as = gameObject.AddComponent<AudioSource>();
//                audioSources.Add(_as);
//            }

//            _audioSources = audioSources.ToArray();

//        }

//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//                PlaySound(testSound);

//            if (Input.GetKeyDown(KeyCode.Backspace))
//                StopSound(testSound);

//            //print("Sound is playing: " + testSound.IsPlaying());

//        }

//        public void PlaySound(Sound _sound)
//        {
//            _sound.Initialize();
//            _sound.Play();
//        }

//        public void StopSound (Sound _sound)
//        {
//            _sound.Stop();
//        }

//        public AudioSource FindAvailableAudioSource()
//        {
//            foreach (AudioSource _as in _audioSources)
//            {
//                if (!_as.isPlaying)
//                {
//                    return _as;
//                }
//            }
            
//            Debug.LogError("No available audio sources found when trying to find one source!");

//            return null;

//        }

//        public AudioSource[] FindAvailableAudioSources(int _numOfSourcesToFind)
//        {
//            List<AudioSource> _availableAudioSources = new List<AudioSource>();
//            int _audioSourceCount = 0;
//            AudioSource[] _sourcesToReturn = null;

//            foreach (AudioSource _as in _audioSources)
//            {
//                if (!_as.isPlaying && _audioSourceCount < _numOfSourcesToFind)
//                {
//                    _availableAudioSources.Add(_as);
//                    _audioSourceCount++;

//                    //if (_audioSourceCount >= _numOfSourcesToFind)
//                    //{
//                    //    _sourcesToReturn = _availableAudioSources.ToArray();
//                    //}

//                }
//            }

//            _sourcesToReturn = _availableAudioSources.ToArray();

//            if (_sourcesToReturn.Length < _numOfSourcesToFind)
//            {
//                _sourcesToReturn = null;
//                Debug.LogError($"No available audio sources found when trying to find {_numOfSourcesToFind}!");
//            }

//            return _sourcesToReturn;

//        }

//    }


//}
