using System.Collections.Generic;
using UnityEngine;

public class PlayParticleFX : MonoBehaviour
{
    [SerializeField] public List<NamedParticleFX> pSystems = new List<NamedParticleFX>();

    //called by animator to play named particle system
    public void PlayParticleSystem(string nameOfSystem)
    {
        foreach (NamedParticleFX p in pSystems)
        {
            if(p.name == nameOfSystem) { p.system.Play(); return; }
        }
    }


    //called by animator to stop named particle system
    public void StopParticleSystem(string nameOfSystem)
    {
        foreach (NamedParticleFX p in pSystems)
        {
            if (p.name == nameOfSystem) { p.system.Stop(); return; }
        }
    }
}

[System.Serializable]
public class NamedParticleFX
{
    public string name;
    public ParticleSystem system;
}