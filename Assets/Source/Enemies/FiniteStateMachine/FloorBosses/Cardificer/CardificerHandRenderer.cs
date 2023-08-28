using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    public class CardificerHandRenderer : MonoBehaviour
    {
        [Tooltip("The rune sprite to use when a hand slot is empty")]
        [SerializeField] private Sprite emptyRuneSprite;
        
        // Component for image UI elements in hand
        List<Image> cardsInHand = new List<Image>();
        
        /// <summary>
        /// Grab all Image components in the UI and initialize them
        /// </summary>
        void Start()
        {
            foreach (Image img in GetComponentsInChildren<Image>())
            {
                cardsInHand.Add(img);
            }
        }

        /// <summary>
        /// Updates the hand to contain the cards currently in it
        /// </summary>
        void FixedUpdate()
        {
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                CardificerDeck.CardificerCard card = CardificerDeck.GetCardFromHand(i);

                cardsInHand[i].sprite = card == null ? emptyRuneSprite : card.runeSprite;
            }
        }
    }
}