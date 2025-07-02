using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossMusic : MonoBehaviour
{

    void Start()
    {
        MusicManager.instance.SetMusicToBossMusic_public();   
    }

    private void OnDestroy()
    {
        MusicManager.instance.SetMusicToAmbientMusicFromBossMusic_public();
    }

}
