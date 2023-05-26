using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
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
        public bool attachedToSpawnLocaiton = true;


        #region Previewing
        public override void ApplyModifiersToPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            throw new System.NotImplementedException();
        }

        public override void CancelPreview(IActor actor)
        {
            throw new System.NotImplementedException();
        }

        public override void Preview(IActor actor)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveModifiersFromPreview(IActor actor, List<AttackModifier> actionModifiers)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public override void Play(IActor actor, List<AttackModifier> modifiers, GameObject causer, List<GameObject> ignoredObjects = null)
        {
            actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(PlaySpawnSequence(actor, modifiers, causer, ignoredObjects));
        }

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

        public enum RotationDirection
        {
            Clockwise,
            Counterclockwise
        }
    }
}
