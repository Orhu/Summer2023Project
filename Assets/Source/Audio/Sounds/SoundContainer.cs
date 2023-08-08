using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;


namespace Cardificer
{
    [CreateAssetMenu(fileName = "NewSoundContainer", menuName = "Sound/Sound Container")]
    public class SoundContainer : Sound
    {
        [SerializeField] private string _name = "NewSoundContainer";
        [SerializeField] private Sound[] _sounds;
        [SerializeField] private SoundContainerType _containerType;
        [SerializeField] private AudioMixerGroup _audioMixerGroup;
        private AudioSource[] _audioSourcesInUse;

        //private int _currentSoundIndex;
        //private int _soundsLength;
        private int _awaitTimeMiliseconds;
        private int _indexOfNextSourceToLoad;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public override string name { get { return _name; } }
        public override SoundType soundType { get { return SoundType.SoundContainer; } }

        private void Awake()
        {
        }

        public override void AssignMixer()
        {
            foreach (AudioSource _as in _audioSourcesInUse)
            {
                _as.outputAudioMixerGroup = _audioMixerGroup;
            }
        }

        public override void Initialize()
        {
            foreach (Sound s in _sounds)
            {
                if (s.soundType == SoundType.BasicSound)
                    s._loop = false;
            }

            //_playingAudio = false;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            switch (_containerType)
            {
                case SoundContainerType.Sequential:

                    SetSequentialDefaults();
                    _audioSourcesInUse = AudioManager_NEW.instance.FindAvailableAudioSources(2);
                    AssignMixer();

                    break;

                case SoundContainerType.RandomSequential:
                    Debug.Log("Initialization of RandomSequential type not implemented");
                    break;

                case SoundContainerType.RandomOneShot:
                    Debug.Log("Initialization of RandomOneShot type not implemented");
                    break;

                case SoundContainerType.RandomBurst:
                    Debug.Log("Initialization of RandomBurst type not implemented");
                    break;

                    #region set default methods
                    void SetSequentialDefaults()
                    {
                        Array.Clear(_audioSourcesInUse, 0, _audioSourcesInUse.Length);
                        //_shouldPlay = true;
                        _indexOfNextSourceToLoad = 1;
                        _awaitTimeMiliseconds = 0;
                    }


                    #endregion

            }
        }

        public override void Initialize(AudioSource _as)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsPlaying()
        {
            bool _playing = false;
            foreach (Sound _s in _sounds)
            {
                if (_s.IsPlaying())
                    _playing = true;
            }

            return _playing;
        }

        public override void Pause()
        {

        }

        public override void Play()
        {

            if (_cancellationToken.IsCancellationRequested)
                return;

            //_shouldPlay = true; //doesnt really stop anything but is important for stopping async methods, should check for other variables (pause, etc)

            PlayContainer(_cancellationToken);

        }

        public void Play(Vector3 _location)
        {
            
        }

        public override void Stop()
        {

            //_shouldPlay = false;
            _cancellationTokenSource.Cancel();
            foreach (AudioSource _as in _audioSourcesInUse)
                _as.Stop();

        }

        private async Task LoadSoundAndPlayDelayed(Sound _sound, AudioSource _sourceToLoad, int _awaitTime, CancellationToken _token)
        {

            //load sound
            _sound.Initialize(_sourceToLoad);

            //set next wait time
            double _processedAwaitTime = Math.Truncate(_sound.GetLength() * 1000);
            //_awaitTimeMiliseconds = _shouldPlay ? (int)_processedAwaitTime : 0;
            _awaitTimeMiliseconds =(int)_processedAwaitTime;

            //Debug.Log($"Sound to play: {_sound.name} \nSource to load: {_sourceToLoad.name} \nWaiting before playing this sound: {_awaitTime} \nNext waiting time: {_awaitTimeMiliseconds}");

            //if (_token.IsCancellationRequested)
            //    return;

            //wait miliseconds
            await Task.Delay(_awaitTime, _token);

            //play sound
            if (!_token.IsCancellationRequested)
            {
                Debug.Log("Sound played");
                _sourceToLoad.Play();

            }
        }

        private async Task PlayContainer(CancellationToken _token)
        {

            if (_token.IsCancellationRequested)
                return;

            //_playingAudio = true;

            switch (_containerType)
            {
                case SoundContainerType.Sequential:

                    _awaitTimeMiliseconds = 0;
                    _indexOfNextSourceToLoad = 0;
                    foreach (Sound _s in _sounds)
                    {
                        await LoadSoundAndPlayDelayed(_s, _audioSourcesInUse[_indexOfNextSourceToLoad], _awaitTimeMiliseconds, _token);
                        _indexOfNextSourceToLoad = _indexOfNextSourceToLoad == 0 ? 1 : 0;
                    }

                    if (_token.IsCancellationRequested)
                        return;

                    Debug.LogWarning($"Container done playing. Looping = {_loop}");

                    //_playingAudio = false;

                    if (_loop && !_token.IsCancellationRequested)
                    {
                        await Task.Delay(_awaitTimeMiliseconds);
                        //Stop();
                        Play();
                    }
                    break;

                case SoundContainerType.RandomSequential:
                    Debug.Log("Play of RandomSequential type not implemented");
                    break;

                case SoundContainerType.RandomRandom:
                    Debug.Log("Play of RandomSequential type not implemented");
                    break;

                case SoundContainerType.RandomOneShot:
                    Debug.Log("Play of RandomOneShot type not implemented");
                    break;

                case SoundContainerType.RandomBurst:
                    Debug.Log("Play of RandomBurst type not implemented");
                    break;
            }
        }

        public override float GetLength()
        {
            float _totalLength = 0;

            foreach (Sound _s in _sounds)
            {
                _totalLength += _s.GetLength();
            }

            return _totalLength;
        }


    }
    public enum SoundContainerType
    {
        Sequential,
        RandomSequential,
        RandomRandom,
        RandomOneShot,
        RandomBurst,
    }

}


