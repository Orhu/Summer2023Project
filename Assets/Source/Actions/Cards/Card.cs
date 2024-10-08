using UnityEngine;
using Skaillz.EditInline;

namespace Cardificer
{
    /// <summary>
    /// A scriptable object for containing data about a card type including:
    /// - Card visuals
    /// - The cooldown of the card
    /// - Actions - What the card does when played as a root
    /// - Effects - How the card effects the dungeon
    /// </summary>
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Cards/Normal Card", order = 1)]
    //[ExecuteInEditMode]
    public class Card : ScriptableObject
    {
        [Header("Mechanics")]

        [Tooltip("The amount of time this card reserves the hand slot for after being played.")]
        public float cooldownTime = 1.0f;

        [Tooltip("The actions that will be taken when this card is played as the root of a combo.")] [EditInline]
        public Action[] actions;



        [Header("Visuals")]

        [Tooltip("The name of the card as shown to the player.")]
        public string displayName = "Unnamed";

        [Tooltip("The description where variable names inside of [] will be replaced with the variable's value when shown to the player.")] [Multiline]
        public string description = "No Description";

        [Tooltip("The flavor text of the card as shown to the players.")] [Multiline]
        public string flavorText;

        [Tooltip("The card specific sprite on the Actions side of the card.")]
        public Sprite actionImage;

        [Tooltip("The general background card sprite on the Actions side of the card.")]
        public Sprite actionBackground;

        [Tooltip("The card specific sprite on the Effects side of the card.")]
        public Sprite effectImage;

        [Tooltip("The general background card sprite on the Effects side of the card.")]
        public Sprite effectBackground;

        [Tooltip("The card specific rune sprite to be displayed on UI")]
        public Sprite runeImage;

        [Tooltip("The card specific sprite to be displayed as the root of a chord in the UI")]
        public Sprite chordImage;

        [Tooltip("The chord specific color of the rune, to be displayed with the chord in the UI")]
        public Color chordColor;

        [Tooltip("Whether a specific card is rare or not, to be displayed in the UI")]
        public bool isRare = false;

        [Tooltip("The card specific sprite representing the type of damage the card does.")]
        public Sprite damageTypeSprite;

        [Tooltip("How much damage a specific card does. Also is displayed in the UI.")]
        public float damage = 1;

        [Tooltip("The text displayed in the card UI explaining what happens when you chord the card.")]
        public string chordEffectText = "Bees";

        [Tooltip("The cost to copy the card in the card printer")]
        public int copyPrice = 5;

        [Tooltip("The cost to shred the card in the card printer")]
        public int shredPrice = 5;


        /// <summary>
        /// Plays all of the actions of this card from the actor.
        /// </summary>
        /// <param name="actor"> The actor that will be playing this action. </param>
        public void PlayActions(IActor actor)
        {

            Debug.Log($"Card played!");

            foreach (Action cardAction in actions)
            {
                cardAction.Play(actor);
            }
        }
    }
}