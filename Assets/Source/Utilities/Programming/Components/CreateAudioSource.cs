using Cardificer;
using UnityEngine;
/// <summary>
/// Creates an audio source and plays on Start()
/// </summary>
public class CreateAudioSource : MonoBehaviour
{
    [Tooltip("The AudioClip for destruction of the object")]
    [SerializeField] private Sound onDestroySound;

    [Tooltip("The AudioClip for the lifetime  of the object")]
    [SerializeField] private Sound lifetimeSound;

    /// <summary>
    /// Gets the audio source component and plays it. 
    /// </summary>
    void Start()
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        AudioManager.instance.ApplySoundSettingsToAudioSource(lifetimeSound, audioSource);
        audioSource.Play();

    }

    /// <summary>
    /// Plays the destruction SFX on destroy
    /// </summary>
    private void OnDestroy()
    {
        AudioManager.instance.PlayAudioAtPos(onDestroySound, transform.position);
    }

   
}
