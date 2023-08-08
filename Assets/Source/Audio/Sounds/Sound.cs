using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{

    abstract public class Sound : ScriptableObject
    {
        [HideInInspector] public abstract new string name { get; }
        [HideInInspector] public abstract SoundType soundType { get; }
        public bool _loop;
        public abstract void Play();
        public abstract void Stop();
        public abstract void Pause();
        public abstract bool IsPlaying();
        public abstract void AssignMixer();
        public abstract void Initialize();
        public abstract void Initialize(AudioSource _as);
        public abstract float GetLength();

    }

    public enum SoundType
    {
        BasicSound,
        SoundContainer,
        Music,
    }

}


