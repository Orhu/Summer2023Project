using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serves as a container to track targets, obstacles, and current target (if any)
/// </summary>
public class AIData : MonoBehaviour
{
    // list of all targets
    public List<Transform> targets = null;
    
    // list of all obstacles
    public Collider2D[] obstacles = null;

    // current target
    public Transform currentTarget;

    // if targets is uninitialized, return 0. otherwise, return how many targets we have
    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
}