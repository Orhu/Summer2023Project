using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class AudioFadeOutOnDestroy : MonoBehaviour
{
    public BasicSound soundToPlayOnStart;
    public float fadeOutTime;

    private AverageAudio averageAudio;

    private void Start()
    {

        averageAudio = AudioManager.instance.CreateAndPlayAverageAudioSource(soundToPlayOnStart);
        averageAudio.listOfTransformsToTrack.Add(transform);

    }

    private void OnDestroy()
    {
        averageAudio.DestroyAverageAudio(fadeOutTime);
    }


}
