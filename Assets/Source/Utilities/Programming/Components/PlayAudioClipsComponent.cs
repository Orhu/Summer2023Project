using UnityEngine;

/// <summary>
/// A specific class for the bomb audio including deployment of the bomb and the explosion of the bomb. 
/// </summary>
public class BombAudio : MonoBehaviour
{
    [Tooltip("The AudioClip for the explosion of the bomb ")]
    [SerializeField] private AudioClip impactAudioClip;

    [Tooltip("The AudioClip for the lifetime of the bomb ")]
    [SerializeField] private AudioClip lifetimeAudioClip;

    /// <summary>
    /// Add an audio source the bomb game object. 
    /// </summary>
    void Start()
    { 
        var audioSource = new GameObject().AddComponent<AudioSource>();

    }

   
}
