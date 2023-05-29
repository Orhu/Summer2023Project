using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Plays the 1st particle system when the bomb detonates and destroys the game object after the system is finished.
/// </summary>
public class BombVisuals : MonoBehaviour
{
    public UnityEvent<float> explosionRadius;
    public UnityEvent<Vector3> explosionRadiusAsScale;
    public UnityEvent<float> fuseTime;
    public UnityEvent onExploded;

    // The remaining lifetime of the visuals.
    private float lifetime;
    // The remaining lifetime of the visuals.
    private new ParticleSystem particleSystem;
    // The remaining lifetime of the visuals.
    private Bomb bomb;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        lifetime = particleSystem.main.duration;
        bomb = GetComponentInParent<Bomb>();

        explosionRadius?.Invoke(bomb.explosionRadius);
        explosionRadiusAsScale?.Invoke(new Vector3(bomb.explosionRadius, bomb.explosionRadius, bomb.explosionRadius));

        if (onExploded != null)
        {
            bomb.onExploded += onExploded.Invoke;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (particleSystem.isEmitting)
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            fuseTime?.Invoke(bomb.fuseTime);
        }
    }
}
