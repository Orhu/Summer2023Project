using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// The obstacle detector detects obstacles
    /// </summary>
    public class ObstacleDetector : Detector
    {
        // radius to detect obstacles
        [SerializeField]
        private float detectionRadius = 2;

        // layer mask that has obstacles on it
        [SerializeField]
        private LayerMask obstacleLayerMask;

        // show gizmos?
        [SerializeField]
        private bool showGizmos = true;

        // tracks colliders
        Collider2D[] colliders;

        /// <summary>
        /// Detects nearby obstacles
        /// </summary>
        /// <param name="aiData">AI data structure to write to</param>
        public override void Detect(AIData aiData)
        {
            colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, obstacleLayerMask);
            aiData.obstacles = colliders;
        }

        private void OnDrawGizmos()
        {
            if (showGizmos == false)
                return;
            if (Application.isPlaying && colliders != null)
            {
                Gizmos.color = Color.red;
                foreach (Collider2D obstacleCollider in colliders)
                {
                    Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
                }
            }
        }
    }
}