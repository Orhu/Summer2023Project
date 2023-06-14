using System.Collections;
using UnityEngine;

/// <summary>
/// Represents an action to update our target to be the player
/// </summary>
[CreateAssetMenu(menuName = "FSM/Actions/Set Target To Player")]
public class SetTargetToPlayer : FSMAction
{
    /// <summary>
    /// Enum representing targeting modes for setting a target
    /// </summary>
    enum TargetType
    {
        Pathfinding,
        Shooting,
        Both
    }

    [Tooltip("Target type to use. Should we set the pathfinding target, attack target, or both for this unit?")]
    [SerializeField] private TargetType targetType;

    public override IEnumerator PlayAction(BaseStateMachine stateMachine)
    {
        switch (targetType)
        {
            case TargetType.Both:
                stateMachine.currentPathfindingTarget = Player.GetFeet().transform.position;
                stateMachine.currentAttackTarget = Player.Get().transform.position;
                break;
            case TargetType.Pathfinding:
                stateMachine.currentPathfindingTarget = Player.GetFeet().transform.position;
                break;
            case TargetType.Shooting:
                stateMachine.currentAttackTarget = Player.Get().transform.position;
                break;
        }

        stateMachine.cooldownData.cooldownReady[this] = true;
        yield break;
    }
}