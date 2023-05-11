using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSystem.Effects
{
    [CreateAssetMenu(fileName = "NewProjectile", menuName = "Cards/Actions/Projectile")]
    public class Projectile : CardAction
    {
        public int damage = 1;
        public float range = 6;
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