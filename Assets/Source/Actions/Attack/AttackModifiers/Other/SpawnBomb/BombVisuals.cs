using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sends out the appropriate unity events that may be needed by bomb VFX, and handles the lifetime of the bomb.
/// </summary>
public class BombVisuals : MonoBehaviour
{
    [Tooltip("Called when the explosion radius is initialized, and passes the explosion radius.")]
    public UnityEvent<float> explosionRadius;

    [Tooltip("Called when the explosion radius is initialized, and passes the explosion radius as a vector 3.")]
    public UnityEvent<Vector3> explosionRadiusAsScale;

    [Tooltip("Called when the when the fuse time is updated, and passes the remaining time.")]
    public UnityEvent<float> fuseTime;

    [Tooltip("Called when the when the bomb explodes.")]
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
