using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CardSystem
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card", order = 1)]
    public class Card : ScriptableObject
    {
        [Header("Visuals")]
        public string displayName = "Unnamed";
        public Sprite actionImage;
        public Sprite actionBackground;
        public Sprite effectImage;
        public Sprite effectBackground;

        [Header("Effects")]
        public CardAction[] cardActions;
        public CardDungeonEffect[] cardEffects;

        public string GetDescription(bool isActionSide)
        {
            string description = "";
            if (!isActionSide)
            {
                foreach (CardDungeonEffect cardEffect in cardEffects)
                {
                    description += cardEffect.GetFormattedDescription() + "\n";
                }
            }
            else
            {
                foreach (CardAction cardAction in cardActions)
                {
                    description += cardAction.GetFormattedDescription() + "\n";
                }
            }
            return description;
        }

        public void PreviewEffect()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.PreviewEffect();
            }
        }

        public void CancelPreview()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.CancelPreview();
            }
        }

        public void ConfirmPreview()
        {
            foreach (CardAction cardAction in cardActions)
            {
                cardAction.ConfirmPreview();
            }
        }
    }
}