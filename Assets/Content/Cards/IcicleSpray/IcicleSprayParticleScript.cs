using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleSprayParticleScript : MonoBehaviour { // make pawticul on destwoy object uwu
    [SerializeField] GameObject onDeathParticlePrefab;

    void OnDestroy() 
    {
        Instantiate(onDeathParticlePrefab, transform.position, transform.rotation);
    }
}