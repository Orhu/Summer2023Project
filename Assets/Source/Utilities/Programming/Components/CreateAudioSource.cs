using Cardificer;
using UnityEngine;
/// <summary>
/// Creates an audio source and plays on Start()
/// </summary>
public class CreateAudioSource : MonoBehaviour
{
    [Tooltip("The AudioClip for destruction of the object")]
    [SerializeField] private AudioClip onDestroyAudioClip;

    [Tooltip("The AudioClip for the lifetime  of the object")]
    [SerializeField] private AudioClip lifetimeAudioClip;

    /// <summary>
    /// Gets the audio source component and plays it. 
    /// </summary>
    void Start()
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = lifetimeAudioClip;
        audioSource.loop = true;
        audioSource.spatialBlend = .5f;
        audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
        audioSource.Play();

    }

    /// <summary>
    /// Plays the destruction SFX on destroy
    /// </summary>
    private void OnDestroy()
    {
        AudioManager.instance.PlayAudioAtPos(onDestroyAudioClip, transform.position);
    }

   
}
