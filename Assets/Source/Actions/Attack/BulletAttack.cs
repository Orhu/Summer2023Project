using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewBulletAttack", menuName = "Cards/Actions/BulletAttack")]
    internal class BulletAttack : Attack
    {
        [Tooltip("The sequence of when and where to spawn bullets")]
        public List<BulletSpawnInfo> spawnSequence = new List<BulletSpawnInfo>();

        [Tooltip("The angle relative to the aim direction that this projectile will spawn at")]
        public float randomAngle = 0;

        [Tooltip("Random ")]
        public Vector2 randomOffset = Vector2.zero;

        [System.Serializable]
        internal class BulletSpawnInfo
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
