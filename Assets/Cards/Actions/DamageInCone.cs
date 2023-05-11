using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewDamageInCone", menuName = "Cards/Actions/DamageInCone")]
    public class DamageInCone : CardAction
    {
        public int damage = 1;
        public override string GetFormattedDescription()
        {
            return description.Replace("[Damage]", damage.ToString());
        }

        public override void PreviewEffect()
        {
            Debug.Log("Hello World");
        }
        public override void CancelPreview()
        {
            Debug.Log("Cancel World");
        }
        public override void ConfirmPreview()
        {
            Debug.Log("Yes World");
        }
    }
}