using Cardificer;
using UnityEngine;
/// <summary>
/// Creates an audio source and plays on Start()
/// </summary>
public class CreateAudioSource : MonoBehaviour
{
    [Tooltip("The AudioClip for destruction of the object")]
    [SerializeField] private BasicSound onDestroySound;

    [Tooltip("The AudioClip for the lifetime  of the object")]
    [SerializeField] private BasicSound lifetimeSound;

    /// <summary>
    /// Gets the audio source component and plays it. 
    /// </summary>
    void Start()
    {

        AudioManager.instance.PlaySoundBaseOnTarget(lifetimeSound, transform, false);

    }

    /// <summary>
    /// Plays the destruction SFX on destroy
    /// </summary>
    private void OnDestroy()
    {
        AudioManager.instance.PlaySoundBaseAtPos(onDestroySound, transform.position, gameObject.name);
    }

   
}
