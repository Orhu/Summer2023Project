using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnObject : MonoBehaviour
{
    public BasicSound soundToPlay;

    void Start()
    {
        AudioManager.instance.PlaySoundBaseOnTarget(soundToPlay, transform, true);
    }
}
