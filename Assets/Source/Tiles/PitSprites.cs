using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// Class that takes in a sprite number and spits out the approprate sprite
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewPitSprites", menuName = "Pits/PitSprites")]
    public class PitSprites : ScriptableObject
    {
        [Tooltip("The order the sprites appear in")]
        [SerializeField] private PitOrder pitOrder;

        [Tooltip("The sprites to use")]
        [SerializeField] private List<Sprite> sprites;

        /// <summary>
        /// Gets a sprite with the given number
        /// </summary>
        /// <param name="spriteNumber"> The sprite number of the sprite </param>
        /// <returns></returns>
        public Sprite GetSprite(int spriteNumber)
        {
            if (pitOrder[spriteNumber] == -1)
            {
                Debug.Log("Pit sprite number " + spriteNumber + " is -1!");
                return sprites[pitOrder[0]];
            }
            return sprites[pitOrder[spriteNumber]];
        }
    }
}