using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Spawns a particle effect with the game object is destroyed.
    /// </summary>
    public class SpawnParticleOnDeath : MonoBehaviour
    {
        [Tooltip("The particle effect to spawn")]
        [SerializeField] GameObject onDeathParticlePrefab;

        /// <summary>
        /// Instantiate the particle effect
        /// </summary>
        void OnDestroy()
        {
            if (!gameObject.scene.isLoaded) { return; }
            Instantiate(onDeathParticlePrefab, transform.position, transform.rotation);
        }
    }
}