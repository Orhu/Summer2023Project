using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class CoinPickup : MonoBehaviour
    {
        [Tooltip("The number of coins to give")] [Min(1)]
        public int coins = 1;

        public BasicSound coinCollectSound;
        private SoundContainer coinSounds;
        private SoundBase coinSoundToPlay;

        private void Start()
        {

            coinSoundToPlay = coinCollectSound;

            if (coins > 2)
            {
                int numClipsToAdd = (int)Mathf.Floor(coins / 2);
                List<AudioClip> clipsToAdd = new List<AudioClip>();

                for (int i = 0; i < numClipsToAdd; i++)
                {
                    clipsToAdd.Add(coinCollectSound.audioClip);
                }

                AudioManager.instance.ApplySoundSettingsToSound(coinCollectSound, coinSounds);
                coinSounds.clipsInContainer = clipsToAdd.ToArray();
                coinSounds.containerType = SoundContainerType.Sequential;

                coinSoundToPlay = coinSounds;

            }
        }

        /// <summary>
        /// Pickup coins.
        /// </summary>
        /// <param name="collision"> If player give coins. </param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                AudioManager.instance.PlaySoundBaseAtPos(coinSoundToPlay, transform.position, gameObject.name);
                Player.AddMoney(coins);
                Destroy(gameObject);
            }
        }
    }
}