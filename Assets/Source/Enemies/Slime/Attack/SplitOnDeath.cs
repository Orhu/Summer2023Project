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
        [SerializeField] private GameObject splitIntoPrefab;

        /// <summary>
        /// Split by creating a number of enemies along the circumference of the circle given by the splitRadius
        /// </summary>
        public void Split()
        {
            var step = 360 / numToSplitInto;
            var myPos = transform.position;

            for (int i = 0; i < numToSplitInto; i++)
            {
                var degree = step * i;
                var xVal = splitRadius * Mathf.Cos(degree) + myPos.x;
                var yVal = splitRadius * Mathf.Sin(degree) + myPos.y;
                var spawnPos = new Vector2(xVal, yVal);

                Instantiate(splitIntoPrefab, spawnPos, Quaternion.identity, transform.parent);
            }
        }
    }
}