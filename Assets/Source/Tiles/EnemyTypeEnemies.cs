using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Stores an enemy type and the enemies that are part of that enemy type
    /// </summary>
    [System.Serializable] [CreateAssetMenu(fileName = "NewEnemyTypeEnemies", menuName = "Generation/EnemyTypeEnemies")]
    public class EnemyTypeEnemies : ScriptableObject
    {
        [Tooltip("The enemy type")]
        public EnemyType enemyType;

        [Tooltip("The enemies that are part of that enemy type")]
        public GenericWeightedThings<GameObject> enemies;
    }

    /// <summary>
    /// The types of enemies (enemies can be multiple types)
    /// </summary>
    [System.Serializable] [System.Flags]
    public enum EnemyType
    {
        None = 0,
        Burrow = 1,
        Fly = 2,
        Walk = 3,
    }
}