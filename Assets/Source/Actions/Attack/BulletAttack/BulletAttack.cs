using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attacks
{
    /// <summary>
    /// An attack that fires a bullet in a strait line.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBulletAttack", menuName = "Cards/Actions/Attacks/Bullet Attack")]
    public class BulletAttack : Attack
    {
        [Tooltip("The sequence of when and where to spawn bullets")]
        public List<BulletSpawnInfo> spawnSequence = new List<BulletSpawnInfo>() { new BulletSpawnInfo() };

        [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
        public float randomAngle = 0f;

        [Tooltip("The radius of the circle to randomly pick a point to spawn projectiles within.")]
        public float randomOffset = 0f;

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

        #region Playing
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
                BulletProjectile bullet = SpawnProjectile(actor, modifiers, causer, ignoredObjects) as BulletProjectile;
                bullet.bulletIndex = i;
            }
        }
        #endregion

        /// <summary>
        /// The information about a single bullet spawning event.
        /// </summary>
        [System.Serializable]
        public class BulletSpawnInfo
        {
            [Tooltip("The time to wait after the previous bullet to spawn this one")]
            public float delay = 0;

            [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
            public float angle = 0;

            [Tooltip("The offset from the spawn location to spawn this bullet at")]
            public Vector2 offset = Vector2.zero;
        }
    }
}
