using System;
using System.Collections;
using System.Collections.Generic;
using Cardificer;
using UnityEngine;

public class CreateAudioSource : MonoBehaviour
{
    [SerializeField] private AudioClip onDestroyAudioClip;

    [SerializeField] private AudioClip lifetimeAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = lifetimeAudioClip;
        audioSource.loop = true;
        audioSource.spatialBlend = .5f;
        audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
        audioSource.Play();

    }

    private void OnDestroy()
    {
        AudioManager.instance.PlayAudioAtPos(onDestroyAudioClip, transform.position);
    }

   
}
