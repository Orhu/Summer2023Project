using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewBulletAttack", menuName = "Cards/Actions/Attacks/BulletAttack")]
    public class BulletAttack : Attack
    {
        [Tooltip("The sequence of when and where to spawn bullets")]
        public List<BulletSpawnInfo> spawnSequence = new List<BulletSpawnInfo>();

        [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
        public float randomAngle = 0;

        [Tooltip("Random ")]
        public Vector2 randomOffset = Vector2.zero;

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
