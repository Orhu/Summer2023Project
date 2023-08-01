using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class Spawner : MonoBehaviour
    {
        [Tooltip("The time between spawning.")]
        [SerializeField] private float spawnFrequncy = 1f;

        [Tooltip(".")]
        [SerializeField] private float spawnDistance = 20f;

        [Tooltip(".")]
        [SerializeField] private Bounds spawnBounds;

        [Tooltip("Things to spawn.")]
        [SerializeField] private List<SpawnPool> spawnPools;

        [System.Serializable]
        private class SpawnPool
        {
            [Tooltip("Things to spawn.")]
            public List<SpawnableInfo> spawnEntries = new List<SpawnableInfo>();

            [Tooltip(".")]
            public AnimationCurve spawnpointsOverTime;

            // 
            public List<SpawnedInfo> spawnedThings = new List<SpawnedInfo>();

            // 
            [System.NonSerialized] public int consumedSpawnPoints = 0;

            [System.Serializable]
            public class SpawnableInfo
            {
                [field: Tooltip(".")]
                [field: SerializeField] public GameObject thingToSpawn { private set; get; }

                [field: Tooltip("The amount of spawn points this takes up while spawned")] [field: Min(1)]
                [field: SerializeField] public int spawnpointCost { private set; get; } = 1;

                [Tooltip(".")]
                [SerializeField] private AnimationCurve weightOverTime;

                public float GetWeight(float currentTime)
                {
                    return weightOverTime.Evaluate(currentTime);
                }
            }

            public class SpawnedInfo
            {
                [Tooltip(".")]
                public GameObject spawnedObject;

                [Tooltip(".")]
                public int spawnPoints;

                public SpawnedInfo(GameObject spawnedObject, int spawnPoints)
                {
                    this.spawnedObject = spawnedObject;
                    this.spawnPoints = spawnPoints;
                }
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SpawnThings());

            IEnumerator SpawnThings()
            {
                float currentTime = 0f;
                while (true)
                {
                    yield return new WaitForSeconds(spawnFrequncy);
                    currentTime += spawnFrequncy;

                    foreach (SpawnPool spawnPool in spawnPools)
                    {
                        UpdateSpawnpoints(spawnPool);
                        SpawnNewObjects(spawnPool, currentTime);
                    }
                }
            }

            void UpdateSpawnpoints(SpawnPool spawnPool)
            {
                for (int i = 0; i < spawnPool.spawnedThings.Count; i++)
                {
                    if (spawnPool.spawnedThings[i].spawnedObject == null)
                    {
                        spawnPool.consumedSpawnPoints -= spawnPool.spawnedThings[i].spawnPoints;
                        spawnPool.spawnedThings.RemoveAt(i);
                        i--;
                    }
                }
            }

            void SpawnNewObjects(SpawnPool spawnPool, float currentTime)
            {
                while (spawnPool.consumedSpawnPoints < Mathf.Round(spawnPool.spawnpointsOverTime.Evaluate(currentTime)))
                {
                    SpawnPool.SpawnableInfo objectToSpawn = null;

                    // Pick thing to spawn.
                    float totalWeight = 0f;
                    foreach (SpawnPool.SpawnableInfo spawnableInfo in spawnPool.spawnEntries)
                    {
                        float weight = spawnableInfo.GetWeight(currentTime);
                        if (weight >= Random.Range(0, weight + totalWeight))
                        {
                            objectToSpawn = spawnableInfo;
                        }

                        totalWeight += weight;
                    }

                    // Spawn thing
                    GameObject spawnedThing = Instantiate(objectToSpawn.thingToSpawn);
                    do
                    {
                        spawnedThing.transform.position = (Vector3)Player.GetFeetPosition() + Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * new Vector3(spawnDistance, 0, 0);
                    } while (!spawnBounds.Contains(spawnedThing.transform.position));

                    // Update spawned things
                    spawnPool.spawnedThings.Add(new SpawnPool.SpawnedInfo(spawnedThing, objectToSpawn.spawnpointCost));
                    spawnPool.consumedSpawnPoints += objectToSpawn.spawnpointCost;
                }
            }
        }
    }
}
