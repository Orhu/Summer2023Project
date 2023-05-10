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
        public Sprite actionImage;
        public Sprite actionBackground;
        public Sprite effectImage;
        public Sprite effectBackground;

        [SerializeField]
        private ComponentLinks links;
        [SerializeField]
        private bool isFlipped;

        public void Update()
        {
            SetFlipped(isFlipped);
        }


        public void SetFlipped(bool newIsFlipped)
        {
            isFlipped = newIsFlipped;

            string description = "";
            if (isFlipped)
            {
                CardDungeonEffect[] cardEffects = GetComponents<CardDungeonEffect>();
                foreach (CardDungeonEffect cardEffect in cardEffects)
                {
                    description += cardEffect.GetFormatedDescription() + "\n";
                }


                links.nameTextBox.text = displayName;
                links.descriptionTextBox.text = description;
                links.cardSprite.sprite = effectImage;
                links.backgroundSprite.sprite = effectBackground;
            }
            else
            {
                CardAction[] cardActions = GetComponents<CardAction>();
                foreach (CardAction cardAction in cardActions)
                {
                    description += cardAction.GetFormatedDescription() + "\n";
                }


                links.nameTextBox.text = displayName;
                links.descriptionTextBox.text = description;
                links.cardSprite.sprite = actionImage;
                links.backgroundSprite.sprite = actionBackground;
            }
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
            public TextMeshPro nameTextBox;
            public TextMeshPro descriptionTextBox;
            public SpriteRenderer backgroundSprite;
            public SpriteRenderer cardSprite;
        }
    }
}