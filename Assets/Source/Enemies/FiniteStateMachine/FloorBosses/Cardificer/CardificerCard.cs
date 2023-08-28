using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{
    [CreateAssetMenu(menuName = "Cards/CardificerCard")]
    public class CardificerCard : ScriptableObject
    {
        [Tooltip("Sprite representing the rune of this card")]
        [SerializeField] public Sprite runeSprite;

        [Tooltip("Action time (duration) of this card")]
        [SerializeField] public float actionTime;

        [Tooltip("What state to enter when this card is played")]
        [SerializeField] public BaseState stateToEnter;

        [Tooltip("What state to enter when the action time is complete")]
        [SerializeField] public BaseState stateToExit;

    }
}