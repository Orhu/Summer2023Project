using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The target detector detects targets
/// </summary>
public class TargetDetector : Detector
{
    [Tooltip("Radius to detect targets")]
    [SerializeField] private float targetDetectionRange = 5;

    [Tooltip("Layer masks for obstacles and players")]
    [SerializeField] private LayerMask obstaclesLayerMask, playerLayerMask;

    [Tooltip("Does this detector need line of sight to detect a target?")]
    [SerializeField] private bool needsLineOfSight;

    [Tooltip("Show gizmos?")]
    [SerializeField] private bool showGizmos = true;

    
    // gizmo parameters
    [HideInInspector] private List<Transform> colliders;

    /// <summary>
    /// Detects nearby targets
    /// </summary>
    /// <param name="aiData">AI data structure to write to</param>
    public override void Detect(AIData aiData)
    {
        // find out if player is near
        Collider2D playerCollider =
            Physics2D.OverlapCircle(transform.position, targetDetectionRange, playerLayerMask);

        // check we even detected a player
        if (playerCollider != null)
        {
            // check if you see the player
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit2D hit =
                Physics2D.Raycast(transform.position, direction, targetDetectionRange, obstaclesLayerMask);

            // if we don't need line of sight, just add to targets
            if (!needsLineOfSight)
            {
                colliders = new List<Transform>() { playerCollider.transform };
            }
            // we need line of sight, so make sure we didn't hit any obstacles with the raycast
            else if (hit.collider == null)
            {
                Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.magenta);
                colliders = new List<Transform>() { playerCollider.transform };
            }
            else
            {
                // if we hit an obstacle, we cannot see the player currently
                colliders = null;
            }
        }
        else
        {
            // we can't see the player
            colliders = null;
        }

        aiData.targets = colliders;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (colliders == null)
            return;
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}