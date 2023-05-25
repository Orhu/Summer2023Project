using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewBulletAttack", menuName = "Cards/Actions/Attacks/BulletAttack")]
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

        public override void Play(IActor actor, List<AttackModifier> modifiers, List<GameObject> ignoredObjects = null)
        {
            actor.GetActionSourceTransform().GetComponent<MonoBehaviour>().StartCoroutine(PlaySpawnSequence(actor, modifiers, ignoredObjects));
        }

        IEnumerator PlaySpawnSequence(IActor actor, List<AttackModifier> modifiers, List<GameObject> ignoredObjects)
        {
            for (int i = 0; i < spawnSequence.Count; i++)
            {
                if (spawnSequence[i].delay > 0)
                {
                    yield return new WaitForSeconds(spawnSequence[i].delay);
                }
                BulletProjectile bullet = SpawnProjectile(actor, modifiers, ignoredObjects) as BulletProjectile;
                bullet.bulletIndex = i;
            }
        }


        [System.Serializable]
        public class BulletSpawnInfo
        {
            [Tooltip("The time to wait after the previous bullet to spawn this one")]
            public float delay = 0;

            [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
            public float angle = 0;

            [Tooltip("The time to wait after the previous bullet to spawn this one")]
            public Vector2 offset = Vector2.zero;
        }
    }
}
