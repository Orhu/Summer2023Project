using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cardificer
{
    public class CardificerHandRenderer : MonoBehaviour
    {
        [Header("Sprites")]
        [Tooltip("The rune sprite to use when a hand slot is empty")]
        [SerializeField] private Sprite emptyRuneSprite;
        
        [Tooltip("Currently Selected Card highlight object")]
        [SerializeField] private Image selectedCardHighlight;

        [Header("Randomly Selecting Card Animation")]
        [Tooltip("How many times to select a card randomly before landing on the final card")]
        [SerializeField] private int numberOfRandomSelections = 3;

        [Tooltip("Delay between random selections")]
        [SerializeField] private float delayBetweenRandomSelections = 1f;
        
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
                CardificerCard card = CardificerDeck.GetCardFromHand(i);

                cardsInHand[i].sprite = card == null ? emptyRuneSprite : card.runeSprite;
            }
        }

        public void AnimateSelectCard()
        {
            StartCoroutine(AnimateCardSelection());
        }

        IEnumerator AnimateCardSelection()
        {
            for (int i = 0; i < numberOfRandomSelections; i++)
            {
                selectedCardHighlight.transform.position =
                    cardsInHand[CardificerDeck.GetRandomPlayableCardIndex()].transform.position;
                yield return new WaitForSeconds(delayBetweenRandomSelections);
            }
            
            SetHighlightToSelectedCard();
        }

        public void SetHighlightToSelectedCard()
        {
            selectedCardHighlight.transform.position = cardsInHand[CardificerDeck.selectedCardIndex].transform.position;
        }
    }
}