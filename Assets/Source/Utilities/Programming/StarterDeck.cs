using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A scriptable object for creating starter decks.
    /// </summary>
    [CreateAssetMenu(fileName= "NewStarterDeck", menuName = "Meta/StarterDeck")]
    public class StarterDeck : ScriptableObject
    {
        [Tooltip("The list of cards in the starter deck")]
        public List<Card> cards;
    }
}
