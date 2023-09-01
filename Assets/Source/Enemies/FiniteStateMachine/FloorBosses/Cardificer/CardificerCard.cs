using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{
    [CreateAssetMenu(menuName = "Cards/CardificerCard")]
    public class CardificerCard : ScriptableObject
    {
        [Tooltip("Name of this card (used for Counterspell and Board Wipe hand checks)")]
        [SerializeField] public string cardName;
        
        [Tooltip("Sprite representing the rune of this card")]
        [SerializeField] public Sprite runeSprite;

        [Tooltip("Action time (duration) of this card")]
        [SerializeField] public float actionTime;

        [Tooltip("What state to enter when this card is played")]
        [SerializeField] public BaseState stateToEnter;

        [Tooltip("What state to enter when the action time is complete")]
        [SerializeField] public BaseState stateToExit;
        
        [Tooltip("Is this card playable?")]
        [SerializeField] public bool playable = true;

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="otherCard"> The card to copy </param>
        public CardificerCard(CardificerCard otherCard)
        {
            cardName = otherCard.cardName;
            runeSprite = otherCard.runeSprite;
            actionTime = otherCard.actionTime;
            stateToEnter = otherCard.stateToEnter;
            stateToExit = otherCard.stateToExit;
            playable = otherCard.playable;
        }
    }
}