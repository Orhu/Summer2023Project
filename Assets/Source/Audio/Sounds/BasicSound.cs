//using UnityEngine;
//using UnityEngine.Audio;


//namespace Cardificer
//{
//    [CreateAssetMenu(fileName = "NewBasicSound", menuName = "Sound/Basic Sound")]
//    public class BasicSound : Sound
//    {

//        [SerializeField] private AudioClip _audioClip;
//        [SerializeField, Range(0, 256)] private int _priority = 128;
//        [SerializeField, Range(0, 1)] private float _volume = 1;
//        //[SerializeField] private Vector2 _volumeRandomRange;
//        [SerializeField, Range(0, 3)] private float _pitch = 1;
//        //[SerializeField] private Vector2 _pitchRandomRange;
//        //[SerializeField] private float _pan;
//        [SerializeField] private float _spatialBlend = 0.5f;
//        [SerializeField] private bool _3DSound;
//        [SerializeField] private bool _oneShot = true;
//        [SerializeField] private bool _useInheritedMixer;
//        //[SerializeField] private bool _ignorePause;
//        //private bool _initialized = false;
//        [HideInInspector] public AudioSource audioSource;
//        [SerializeField] private AudioMixerGroup _audioMixerGroup;

//        public override string name { get { return _audioClip.name; } }
//        public override SoundType soundType { get { return SoundType.BasicSound; } }

//        public override void AssignMixer()
//        {
//            audioSource.outputAudioMixerGroup = _audioMixerGroup;
//        }

//        public override void Initialize()
//        {
//            audioSource = AudioManager_NEW.instance.FindAvailableAudioSource();
//            if (audioSource.clip = _audioClip)
//            {
//                //Debug.Log(name + $"Already loaded into {audioSource}");
//                return;
//            }

//            AssignMixer();
//            AssignValuesToAudioSource();
//        }

//        public override void Initialize(AudioSource _as)
//        {
//            audioSource = _as;
//            if (audioSource.clip = _audioClip)
//            {
//                //Debug.Log(name + $"Already loaded into {audioSource}");
//                return;
//            }

//            AssignValuesToAudioSource();
//        }

//        private void AssignValuesToAudioSource()
//        {
//            audioSource.priority = _priority;
//            audioSource.clip = _audioClip;
//            audioSource.volume = _volume;
//            audioSource.pitch = _pitch;
//            audioSource.loop = _loop;
//            audioSource.spatialBlend = _3DSound ? _spatialBlend : 0f;
//        }

//        public override bool IsPlaying()
//        {

//            if (audioSource != null)
//                return audioSource.isPlaying;
//            else
//                return false;

//        }

//        public override void Pause()
//        {
//            if (audioSource != null)
//                audioSource.Pause();
//        }

//        public override void Play()
//        {
//            if (audioSource == null)
//                return;

//            if (_oneShot)
//                audioSource.PlayOneShot(_audioClip);
//            else
//                audioSource.Play();
//        }

//        public void Play(Vector3 _location)
//        {
//            AudioSource.PlayClipAtPoint(_audioClip, _location);
//        }

//        public override void Stop()
//        {
//            audioSource.Stop();
//        }

//        public override float GetLength()
//        {
//            return _audioClip.length;
//        }
//    }
//}

