using Cardificer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnDestroy : MonoBehaviour
{

    [SerializeField] private BasicSound soundToPlayOnDestroy;

    private void OnDestroy()
    {
        AudioManager.instance.PlaySoundBaseAtPos(soundToPlayOnDestroy, transform.position, gameObject.name);
    }
}
