using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Used to render the damage of this.
    /// </summary>
    public class DamageNumber : MonoBehaviour
    {
        // The damage this is showing.
        private DamageData _damageData;
        public DamageData damageData
        {
            set
            {
                GetComponentInChildren<TMP_Text>().text = value.ToString();
                GetComponentInChildren<Animator>().SetInteger("Damage", value.damage);
                _damageData = value;
            }
            get => _damageData;
        }
    }
}