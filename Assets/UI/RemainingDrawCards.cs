using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RemainingDrawCards : MonoBehaviour
{
    public int offset = 0;
    private TMP_Text textBox;
    // Start is called before the first frame update
    void Start()
    {
        textBox = gameObject.GetComponent<TMP_Text>();
        DeckManager.playerDeck.onCardDrawn += OnCardDrawn;
        GetComponentInParent<UnityEngine.UI.VerticalLayoutGroup>().enabled = false;
        Invoke("RefreshParent", 0.1f);
        OnCardDrawn();
    }

    void OnCardDrawn()
    {
        if (DeckManager.playerDeck.drawableCards != null)
        {
            textBox.text = "+" + Mathf.Max(0, DeckManager.playerDeck.drawableCards.Count - offset);
        }
    }

    private void RefreshParent()
    {
        GetComponentInParent<UnityEngine.UI.VerticalLayoutGroup>().enabled = true;
    }
}
