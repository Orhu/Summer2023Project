using System.Collections;
using System.Collections.Generic;
using Cardificer.FiniteStateMachine;
using UnityEngine;

namespace Cardificer
{
    public class CardificerDeck : MonoBehaviour
    {
        [CreateAssetMenu(menuName="Cards/CardificerCard")]
        private class CardificerCard : ScriptableObject
        {
            [Tooltip("Sprite representing the rune of this card")]
            [SerializeField] private Sprite runeSprite;

            [Tooltip("Action time (duration) of this card")]
            [SerializeField] private float actionTime;

            [Tooltip("What state to enter when this card is played")]
            [SerializeField] private BaseState stateToEnter;
            
            [Tooltip("What state to enter when the action time is complete")]
            [SerializeField] private BaseState stateToExit;
        }

        [SerializeField] private List<CardificerCard> cardificerDeck;

        private List<CardificerCard> cardificerHand;

        private List<CardificerCard> discardPile;
    }
}