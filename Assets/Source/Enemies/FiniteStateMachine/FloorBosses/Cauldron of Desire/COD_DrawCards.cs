using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

            [Tooltip("Amount to delay between each action")]
            public List<float> actionDelaySequence;
        }
        
        // tracks seeded random from save manager
        private Random random;

        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("Random", new Random(SaveManager.savedFloorSeed));
            random = stateMachine.trackedVariables["Random"] as Random;
            
            stateMachine.trackedVariables.TryAdd("CardsDrawn", 0);
            stateMachine.trackedVariables.TryAdd("CardsDisplayed", 0);
            
            stateMachine.trackedVariables.Remove("Card"  + stateMachine.trackedVariables["CardsDrawn"]); // if the card exists already, remove it

            var selectedAttackSequence = PickRandomAttack();
            
            stateMachine.trackedVariables.Add("Card" + (int)stateMachine.trackedVariables["CardsDrawn"], selectedAttackSequence);
            stateMachine.trackedVariables["CardsDrawn"] = (int)stateMachine.trackedVariables["CardsDrawn"] + 1;
            
            var cardDisplay = DisplayCard(stateMachine, selectedAttackSequence);
            yield return new WaitForSeconds(displayCardTime);
            Destroy(cardDisplay);
            stateMachine.trackedVariables["CardsDisplayed"] = (int)stateMachine.trackedVariables["CardsDisplayed"] + 1;

            stateMachine.cooldownData.cooldownReady[this] = true;
        }

        private AttackSequence PickRandomAttack()
        {
            AttackSequence randomAttackSequence = cardDrawPool[random.Next(cardDrawPool.Count)];
            return randomAttackSequence;
        }

        private GameObject DisplayCard(BaseStateMachine stateMachine, AttackSequence cardToDisplay)
        {
            var gameObject = new GameObject();
            gameObject.transform.name = "CardPickDisplay";
            var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cardToDisplay.abilitySprite;
            gameObject.transform.position = stateMachine.transform.position + (Vector3.up * 2f);
            var rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.velocity = Vector3.up;
            rigidbody.isKinematic = true;
            gameObject.SetActive(true);
            return gameObject;
        }
    }
}