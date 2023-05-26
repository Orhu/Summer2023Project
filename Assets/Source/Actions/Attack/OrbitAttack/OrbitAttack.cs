using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// An attack that creates a projectile that orbits around the spawn location.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOrbitAttack", menuName = "Cards/Actions/Attacks/Orbit Attack")]
    public class OrbitAttack : Attack
    {
        [Tooltip("The sequence of when and where to spawn bullets")]
        public List<OrbitSpawnInfo> spawnSequence = new List<OrbitSpawnInfo>() { new OrbitSpawnInfo() };

        [Tooltip("The amount the starting angle will vary by")]
        public float randomStartingAngle = 0f;

        [Tooltip("The amount the radius will vary by")]
        public float randomRadius = 0f;

        [Tooltip("Whether or not the projectiles will follow the spawn location")]
        public bool attachedToSpawnLocation = true;


        #region Previewing
        /// <summary>
        /// Starts rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public override void Preview(IActor actor)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Applies modifiers to a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing. </param>
        /// <param name="actionModifiers"> The modifiers to apply </param>
        public override void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes modifiers from a preview.
        /// </summary>
        /// <param name="actor"> The actor previewing. </param>
        /// <param name="actionModifiers"> The modifiers to remove </param>
        public override void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Stops rendering a preview of what this action will do.
        /// </summary>
        /// <param name="actor"> The actor that will no longer be playing this action. </param>
        public override void CancelPreview(IActor actor)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Plays this action and causes all its effects. Also cancels any relevant previews.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        /// <param name="modifiers"> The modifiers to be applied to this attack. </param>
        /// <param name="causer"> The causer of damage dealt by this attack. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        public override void Play(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects = null)
        {
            actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(PlaySpawnSequence(actor, modifiers, causer, ignoredObjects));
        }

        /// <summary>
        /// Spawns all of the projectiles in spawnSequence and creates delays appropriately.
        /// </summary>
        /// <param name="actor"> The actor that is playing this action. </param>
        /// <param name="modifiers"> The modifiers that are applied to this attack. </param>
        /// <param name="causer"> The causer of damage dealt by this attack. </param>
        /// <param name="ignoredObjects"> The objects this action will ignore. </param>
        IEnumerator PlaySpawnSequence(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects)
        {
            for (int i = 0; i < spawnSequence.Count; i++)
            {
                if (spawnSequence[i].delay > 0)
                {
                    yield return new WaitForSeconds(spawnSequence[i].delay);
                }
                OrbitProjectile orbit = SpawnProjectile(actor, modifiers, causer, ignoredObjects) as OrbitProjectile;
                orbit.orbitIndex = i;
            }
        }

        /// <summary>
        /// The information about a single bullet spawning event.
        /// </summary>
        [System.Serializable]
        public class OrbitSpawnInfo
        {
            [Tooltip("The time to wait after the previous bullet to spawn this one")]
            public float delay = 0;

            [Tooltip("The angle on the orbit that projectiles will start at relative to the aim direction")]
            public float startingAngle = 0f;

            [Tooltip("The radius of the orbit")]
            public float radius = 0f;

            [Tooltip("The direction this projectile will orbit in")]
            public RotationDirection orbitDirection;
        }

        /// <summary>
        /// The direction a projectile orbits in.
        /// </summary>
        public enum RotationDirection
        {
            Clockwise,
            Counterclockwise
        }
    }
}
