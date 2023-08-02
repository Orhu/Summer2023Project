using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace Cardificer
{
    [CreateAssetMenu(fileName = "NewBasicSound", menuName = "Sound/Basic Sound")]
    public class BasicSound : Sound
    {

        [SerializeField] private AudioClip _audioClip;
        [SerializeField, Range(0, 256)] private int _priority = 128;
        [SerializeField, Range(0, 1)] private float _volume = 1;
        //[SerializeField] private Vector2 _volumeRandomRange;
        [SerializeField, Range(0, 3)] private float _pitch = 1;
        //[SerializeField] private Vector2 _pitchRandomRange;
        //[SerializeField] private float _pan;
        [SerializeField] private bool _loop;
        [SerializeField] private bool _3DSound;
        [SerializeField] private bool _oneShot = true;
        [SerializeField] private bool _useInheritedMixer;
        //[SerializeField] private bool _ignorePause;
        //private bool _initialized = false;
        [HideInInspector] public AudioSource audioSource;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        public override string name { get { return _audioClip.name; } }

        public override void AssignMixer()
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public override void Initialize(AudioSource _audioSource)
        {
            audioSource = _audioSource;

            audioSource.priority = _priority;
            audioSource.clip = _audioClip;
            audioSource.volume = _volume;
            audioSource.pitch = _pitch;
            audioSource.loop = _loop;
            audioSource.spatialBlend = _3DSound ? 0.7f : 0f;
            AssignMixer();
        }

        public override bool IsPlaying()
        {
            return audioSource.isPlaying;
        }

        public override void Pause()
        {
            audioSource.Pause();
        }

        public override void Play()
        {
            if (_oneShot)
                audioSource.PlayOneShot(_audioClip);
            else
                audioSource.Play();
        }

        public void Play(Vector3 _location)
        {
            AudioSource.PlayClipAtPoint(_audioClip, _location);
        }

        public override void Stop()
        {
            audioSource.Stop();
        }
    }
}

