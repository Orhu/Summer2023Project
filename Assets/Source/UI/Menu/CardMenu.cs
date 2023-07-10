using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Cardificer
{
    /// <summary>
    /// Card manager script for handling rendering and logic
    /// for the card menu UI object
    /// </summary>
    public class CardMenu : MonoBehaviour
    {
        [Tooltip("List of cardRender UI elements for displaying booster pack cards")]
        public List<CardRenderer> cardRenderers = new List<CardRenderer>();

        // UI area for adding cardRenderer UI objects
        [SerializeField] private GameObject scrollCardLayoutArea;

        // The card renderer prefab to instantiate.
        public CardRenderer cardRendererTemplate;

        // Update is called once per frame
        void Update()
        {
            if(scrollCardLayoutArea.transform.childCount != Deck.playerDeck.cards.Count)
            {
                ResetScrollCardLayoutArea();
                InstantiateCardLayoutArea();
            }
        }

        void ResetScrollCardLayoutArea()
        {
            foreach (Transform child in scrollCardLayoutArea.transform)
            {
                Destroy(child.gameObject);
            }
        }

        void InstantiateCardLayoutArea()
        {
            for (int i = 0; i < Deck.playerDeck.cards.Count; i++)
            {
                // Instantiate the cardRenderer Game Object in the cardLayout area
                GameObject tempCardRendererGameObject = Instantiate(cardRendererTemplate.gameObject, scrollCardLayoutArea.transform);

                // Set the smaller scale of the CardRenderer
                tempCardRendererGameObject.GetComponent<CardRenderer>().SetScale(new Vector3(0.85f, 0.9f, 1));

                // Set the scaling factor of the CardRenderer's animation
                tempCardRendererGameObject.GetComponent<CardRenderer>().scaleFactor = 1.05f;

                // Set the scaling duration of the CardRenderer's animation
                tempCardRendererGameObject.GetComponent<CardRenderer>().scaleDuration = 0.25f;

                // Assign the game object a card.
                tempCardRendererGameObject.GetComponent<CardRenderer>().card = Deck.playerDeck.cards[i];

                // Allows for toggling in groups of card renderers
                tempCardRendererGameObject.GetComponent<Toggle>().group = scrollCardLayoutArea.GetComponent<ToggleGroup>();

                // Adds the cardRenderer to the list of cardRenderers
                cardRenderers.Add(tempCardRendererGameObject.GetComponent<CardRenderer>());
            }
        }
    }
}