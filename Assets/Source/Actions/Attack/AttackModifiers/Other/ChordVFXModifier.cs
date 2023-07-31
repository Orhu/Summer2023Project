using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Adds chord VFX to the projectile.
    /// </summary>
    public class ChordVFXModifier : AttackModifier
    {
        // The prefabs to attach.
        [HideInInspector] public List<GameObject> cordVFXPrefabs;

        /// <summary>
        /// Initializes this modifier on the given projectile
        /// </summary>
        /// <param name="attachedProjectile"> The projectile this modifier is attached to. </param>
        public override void Initialize(Projectile attachedProjectile)
        {
            foreach (GameObject chordVFXPrefab in cordVFXPrefabs)
            {
                Instantiate(chordVFXPrefab).transform.SetParent(attachedProjectile.visualObject.transform, false);
            }
        }
    }
}