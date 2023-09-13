using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a sound when destroyed.
/// </summary>
public class PlaySoundOnDestroy : MonoBehaviour
{
    [Header("Sound to play when this component is destroyed")]
    [SerializeField] private BasicSound soundToPlayOnDestroy;


    /// <summary>
    /// Play a BasicSound when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        AudioManager.instance.PlaySoundBaseAtPos(soundToPlayOnDestroy, transform.position, gameObject.name);
    }
}
