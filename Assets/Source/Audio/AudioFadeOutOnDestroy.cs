using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
/// Intantiates an AverageAudio that follows this GameObject and plays a BasicSound on Start(), then fades out and destroys the AverageAudio when this GameObject is destroyed.
/// </summary>
public class AudioFadeOutOnDestroy : MonoBehaviour
{
    [Header("The BasicSound to play on Start()")]
    public BasicSound soundToPlayOnStart;

    [Header("How fast to fade out the playing BasicSound when this GameObject is destroyed")]
    public float fadeOutTime;

    //A reference to the AverageAudio object created
    private AverageAudio averageAudio;

    /// <summary>
    /// Instantiate the AverageAudio GameObject and play the BasicSound.
    /// </summary>
    private void Start()
    {

        averageAudio = AudioManager.instance.CreateAndPlayAverageAudioSource(soundToPlayOnStart);
        averageAudio.listOfTransformsToTrack.Add(transform);

    }

    /// <summary>
    /// Fade and destroy the AverageAudio GameObject.
    /// </summary>
    private void OnDestroy()
    {
        averageAudio.DestroyAverageAudio(fadeOutTime);
    }


}
