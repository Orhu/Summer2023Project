using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CardSystem
{
    [ExecuteInEditMode]
    public class Card : MonoBehaviour
    {
        public string displayName = "Unnamed";
        public Sprite image;
        public Sprite background;

        [SerializeField]
        private ComponentLinks links;

        public void Update()
        {
            string description = "";
            CardAction[] cardEffects = GetComponents<CardAction>();
            foreach (CardAction eachCardEffects in cardEffects)
            {
                description += eachCardEffects.GetFormatedDescription() + "\n";
            }

            links.nameTextBox.text = displayName;
            links.descriptionTextBox.text = description;
            links.cardSprite.sprite = image;
            links.backgroundSprite.sprite = background;
        }

        public void PreviewEffect()
        {
            CardAction[] cardEffects = GetComponents<CardAction>();
            foreach (CardAction eachCardEffects in cardEffects)
            {
                eachCardEffects.PreviewEffect();
            }
        }

        public void CancelPreview()
        {
            CardAction[] cardEffects = GetComponents<CardAction>();
            foreach (CardAction eachCardEffects in cardEffects)
            {
                eachCardEffects.CancelPreview();
            }
        }

        public void ConfirmPreview()
        {
            CardAction[] cardEffects = GetComponents<CardAction>();
            foreach (CardAction eachCardEffects in cardEffects)
            {
                eachCardEffects.ConfirmPreview();
            }
        }

        [System.Serializable]
        private struct ComponentLinks
        {
            public TextMeshProUGUI nameTextBox;
            public TextMeshProUGUI descriptionTextBox;
            public SpriteRenderer backgroundSprite;
            public SpriteRenderer cardSprite;
        }
    }
}