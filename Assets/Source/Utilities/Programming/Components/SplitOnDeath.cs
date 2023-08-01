using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// This SplitOnDeath component facilitates splitting upon death behavior
    /// </summary>
    public class SplitOnDeath : MonoBehaviour
    {
        [Tooltip("How many enemies to create upon death")]
        [SerializeField] private int numToSplitInto;

        [Tooltip("How big of a radius do we split into?")]
        [SerializeField] private float splitRadius;

        [Tooltip("The prefab to spawn upon death")]
        [SerializeField] private FiniteStateMachine.BaseStateMachine splitIntoPrefab;

        [Tooltip("The layers to ensure are empty when spawning")]
        [SerializeField] private LayerMask spawnLayer;

        // The number of attempts to find an appropriate spawn location
        private const int LOCATION_SEACH_ATTEMPTS = 12;

        /// <summary>
        /// Split by creating a number of enemies along the circumference of the circle given by the splitRadius
        /// </summary>
        public void Split()
        {
            float step = 360f / (LOCATION_SEACH_ATTEMPTS * numToSplitInto);
            Vector2 myPos = transform.position;
            List<Vector2> possibleSpawnLocations = new List<Vector2>();

            for (int i = 0; i < LOCATION_SEACH_ATTEMPTS * numToSplitInto; i++)
            {
                float degree = step * i;
                Vector2 direction = new Vector2(Mathf.Cos(degree), Mathf.Sin(degree));
                CapsuleCollider2D feetCapsule = splitIntoPrefab.GetComponentInChildren<CapsuleCollider2D>();
                RaycastHit2D castResult = Physics2D.CapsuleCast(myPos, feetCapsule.size, feetCapsule.direction, feetCapsule.transform.eulerAngles.z, direction, splitRadius, spawnLayer);
                possibleSpawnLocations.Add(castResult ? castResult.centroid : myPos + direction * splitRadius);
            }
            
            possibleSpawnLocations.Sort(
                // Sort the array so the furthest locations are first
                (Vector2 a, Vector2 b) => 
                {
                    float aDist = (a - myPos).sqrMagnitude;
                    float bDist = (b - myPos).sqrMagnitude;
                    return (int)Mathf.Sign(bDist - aDist); 
                });

            possibleSpawnLocations = possibleSpawnLocations.TakeWhile(
                // Remove locations that are not at the maximum distance
                (Vector2 possibleSpawnLocation, int index) =>
                {
                    float dist = Vector2.Distance(possibleSpawnLocation, myPos);
                    return index <= 1 || dist == splitRadius;
                }).ToList();

            for (int i = 0; i < numToSplitInto; i++)
            {
                int randomIndex = Random.Range(0, possibleSpawnLocations.Count);
                Instantiate(splitIntoPrefab.gameObject, possibleSpawnLocations[randomIndex], Quaternion.identity, transform.parent);
                possibleSpawnLocations.RemoveAt(randomIndex);
            }
        }
    }
}