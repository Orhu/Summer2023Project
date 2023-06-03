using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that holds a list of enemy pools
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "NewEnemyPool", menuName = "Generation/EnemyPools", order = 1)]
public class EnemyPools : ScriptableObject
{
    [Tooltip("The enemy pools")]
    public List<EnemyPool> enemyPools;
}

/// <summary>
/// Holds a list of enemies
/// </summary>
[System.Serializable]
public struct EnemyPool
{
    [Tooltip("The enemies in this pool")]
    public List<GameObject> enemies;
}