using TMPro;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MoneyCounter : MonoBehaviour
    {
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
