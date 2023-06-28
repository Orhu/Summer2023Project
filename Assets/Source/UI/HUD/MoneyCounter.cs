using TMPro;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Makes a text box display the player's money.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MoneyCounter : MonoBehaviour
    {
        /// <summary>
        /// Binds the texts to display the player's current money.
        /// </summary>
        private void Start()
        {
            TextMeshProUGUI textBox = GetComponent<TextMeshProUGUI>();
            Player.onMoneyChanged += 
                // Sets the text of the text box to the player's money.
                () => 
                { 
                    textBox.text = Player.GetMoney().ToString(); 
                };

            textBox.text = Player.GetMoney().ToString();
        }
    }
}
