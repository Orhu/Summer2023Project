using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component for exposing particle system properties to unity events.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemPropertyBinder : MonoBehaviour
{

    // The remaining lifetime of the visuals.
    private new ParticleSystem particleSystem;

    // The remaining lifetime of the visuals.
    private ParticleSystem.MainModule mainModule;

    /// <summary>
    /// Initializes references
    /// </summary>
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        mainModule = particleSystem.main;
    }

    // The initial speed of particles when the Particle System first spawns them.
    public float StartSpeed
    {
        set 
        {
            mainModule.startSpeed = value;
        }
        get
        {
            return mainModule.startSpeed.constant;
        }
    }
}
