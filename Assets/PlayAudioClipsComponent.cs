using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAudio : MonoBehaviour
{
    [SerializeField] private AudioClip impactAudioClip;

    [SerializeField] private AudioClip lifetimeAudioClip;
    // Start is called before the first frame update
    void Start()
    { 
        var audioSource = new GameObject().AddComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
