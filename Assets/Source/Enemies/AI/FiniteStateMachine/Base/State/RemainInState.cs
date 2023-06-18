using UnityEngine;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents a state in a finite state machine that simply says "Remain in whatever state you are in" when a transition specifies this class
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Remain In State", fileName = "RemainInState")]
    public sealed class RemainInState : BaseState { }
}