using Cardificer;
using UnityEngine;

/// <summary>
/// Component that makes this GameObject change its Tile to walkable-enabled when it is destroyed.
/// </summary>
public class UpdateGridOnDestroy : MonoBehaviour
{
    [Tooltip("What types of movement are enabled when this object is destroyed?")]
    [SerializeField] private RoomInterface.MovementType enabledMovementTypes;
    
    /// <summary>
    /// Grabs the tile at this GameObject's position, and allows walking on it.
    /// OnDisable is called before the GameObject is fully destroyed. We need its transform, so we use OnDisable.
    /// </summary>
    private void OnDisable()
    {
        (PathfindingTile, bool) grabbedTile = RoomInterface.instance.WorldPosToTile(transform.position, RoomInterface.MovementType.Walk);
        if (grabbedTile.Item2)
        {
            grabbedTile.Item1.allowedMovementTypes |= RoomInterface.MovementType.Walk;
        }
        else
        {
            Debug.LogWarning("Attempted to update grid on destroy, but Tile at " + transform.position + " does not exist.");
        }
    }
}
