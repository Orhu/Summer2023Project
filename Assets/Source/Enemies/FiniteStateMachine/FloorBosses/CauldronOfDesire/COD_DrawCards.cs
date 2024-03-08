using System.Collections;
using System.Collections.Generic;
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
        //Note:Emmeline - can edit this to trigger animations
        [SerializeField] private GenericWeightedThings<AttackSequence> cardDrawPool;
        
        /// <summary>
        /// An attack sequence and its sprite
        /// </summary>
        [System.Serializable]
        public class AttackSequence
        {
            [Tooltip("The sprite that represents this attack sequence")] 
            public Sprite abilitySprite;

            [Tooltip("List of actions to be performed")]
            public List<Attack> actionSequence;
        }
        
        // Tracks seeded random from save manager
        private Random random;

        /// <summary>
        /// Draws a random card, displays it to the player, and updates CardsDrawn and CardsDisplayed variables accordingly.
        /// This ScriptableObject also serves as the draw pool itself.
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <returns> Waits cardDisplayTime before incrementing CardsDisplayed & destroying the display game object. </returns>
        protected override IEnumerator PlayAction(BaseStateMachine stateMachine)
        {
            stateMachine.trackedVariables.TryAdd("Random", new Random(SaveManager.savedFloorSeed));
            random = stateMachine.trackedVariables["Random"] as Random;
            
            stateMachine.trackedVariables.TryAdd("CardsDrawn", 0);
            stateMachine.trackedVariables.TryAdd("CardsDisplayed", 0);
            
            stateMachine.trackedVariables.Remove("Card"  + stateMachine.trackedVariables["CardsDrawn"]); // if the card exists already, remove it

            AttackSequence selectedAttackSequence = PickRandomAttack();
            
            stateMachine.trackedVariables.Add("Card" + (int)stateMachine.trackedVariables["CardsDrawn"], selectedAttackSequence);
            stateMachine.trackedVariables["CardsDrawn"] = (int)stateMachine.trackedVariables["CardsDrawn"] + 1;
            
            GameObject cardDisplay = DisplayCard(stateMachine, selectedAttackSequence);
            yield return new UnityEngine.WaitForSeconds(displayCardTime);
            Destroy(cardDisplay);
            stateMachine.trackedVariables["CardsDisplayed"] = (int)stateMachine.trackedVariables["CardsDisplayed"] + 1;

            stateMachine.cooldownData.cooldownReady[this] = true;
        }

        /// <summary>
        /// Selects a random attack sequence from the card draw pool and returns it
        /// </summary>
        /// <returns> The randomly selected attack sequence. </returns>
        private AttackSequence PickRandomAttack()
        {
            AttackSequence randomAttackSequence = cardDrawPool.GetRandomThing(random);
            return randomAttackSequence;
        }

        /// <summary>
        /// Summons a game object that displays the icon associated with the given attack sequence
        /// </summary>
        /// <param name="stateMachine"> The state machine to be used. </param>
        /// <param name="cardToDisplay"> The attack sequence to be displayed. </param>
        /// <returns> The display game object. </returns>
        private GameObject DisplayCard(BaseStateMachine stateMachine, AttackSequence cardToDisplay)
        {
            //TODO Emmeline
            GameObject gameObject = new GameObject();
            gameObject.transform.name = "CardPickDisplay";
            gameObject.transform.position = stateMachine.transform.position + (Vector3.up * 2f); 
            
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = cardToDisplay.abilitySprite;

            Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.velocity = Vector3.up;
            rigidbody.isKinematic = true;
            
            gameObject.SetActive(true);
            return gameObject;
        }
    }
}