using System;
using System.Collections;
using System.Collections.Generic;
using Skaillz.EditInline;
using UnityEngine;
using Random = System.Random;

namespace Cardificer.FiniteStateMachine
{
    /// <summary>
    /// Represents an action that draws some number of cards and adds them to the state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "FSM/Floor Boss/Cauldron of Desire/Draw Cards")]
    public class COD_DrawCards : SingleAction
    {
        [Tooltip("How long to display a card before drawing the next")]
        [SerializeField] private float displayCardTime;
        
        [Tooltip("Number of cards to draw")]
        [SerializeField] private int numberCardsToDraw;

        [Tooltip("Possible actions that could be drawn to play")]
        [SerializeField] private List<AttackSequence> cardDrawPool;
        
        /// <summary>
        /// An attack sequence and its sprite
        /// </summary>
        [System.Serializable]
        public class AttackSequence
        {
            [Tooltip("The sprite that represents this attack sequence")] 
            public Sprite abilitySprite;

            [Tooltip("List of actions to be performed")]
            public List<Action> actionSequence;
        }
        
        // tracks seeded random from save manager
        private Random random;

        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("Random", new Random(SaveManager.savedFloorSeed));
            random = stateMachine.trackedVariables["Random"] as Random;
            
            stateMachine.trackedVariables.TryAdd("CardsDrawn", 0);
            stateMachine.trackedVariables["CardsDrawn"] = 0; // if it was already added, the TryAdd wont update the value

            stateMachine.trackedVariables.TryAdd("CardsToDraw", numberCardsToDraw); // only needs to be added once, never modified
            
            for (int i = 0; i < numberCardsToDraw; i++)
            {
                stateMachine.trackedVariables.Remove("Card" + i); // try removing it if it exists already
                
                var selectedCard = PickRandomAttack();
                stateMachine.trackedVariables.Add("Card" + i, selectedCard);
                // display selectedCard
                BaseStateMachine.print("Displaying Card " + i);
                yield return new WaitForSeconds(displayCardTime);
                stateMachine.trackedVariables["CardsDrawn"] = (int)stateMachine.trackedVariables["CardsDrawn"] + 1;
            }
            
            stateMachine.cooldownData.cooldownReady[this] = true;
        }

        private AttackSequence PickRandomAttack()
        {
            AttackSequence randomAttackSequence = cardDrawPool[random.Next(cardDrawPool.Count)];
            return randomAttackSequence;
        }
    }
}