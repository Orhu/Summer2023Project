using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A class to leverage average audio positions when projectiles are involved in a card. 
    /// </summary>
    public class AverageAudio : MonoBehaviour
    {
        [Tooltip("The list of spawned projectiles")]
        public List<Projectile> projectiles = new List<Projectile>();

        [Tooltip("The AudioClip to play at the averaged location")]
        public Sound averageSound;


        /// <summary>
        /// Adding an audio source and playing it after adjusting parameters
        /// </summary>
        public void PlayAverageAudio()
        {

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.instance.ApplySoundSettingsToAudioSource(averageSound, audioSource);
            audioSource.Play();
        }

        /// <summary>
        /// Get the average transform of the projectiles
        /// </summary>
        /// <returns>Returns the average position of a list of projectiles</returns>
        public Vector2 TryGetAveragePos()
        {
            float totalX = 0;
            float totalY = 0;

            foreach (var projectile in projectiles)
            {
                if (projectile != null)
                {
                    totalX += projectile.transform.position.x;
                    totalY += projectile.transform.position.y;
                }
            }

            if (totalX == 0 && totalY == 0)
            {
                AudioManager.KillAverageAudio(this);
            }

            return new Vector2(totalX / projectiles.Count, totalY / projectiles.Count);
        }

        /// <summary>
        /// Set the position of the GameObject to the average position of the projectiles. 
        /// </summary>
        private void FixedUpdate()
        {
            transform.position = TryGetAveragePos();

            if (!averageSound.IsPlaying())
            {
                AudioManager.KillAverageAudio(this);
            }

        }

        /// <summary>
        /// Sets the list of Projectiles and the Sound used for this AverageAudio. 
        /// </summary>
        /// <param name="projectiles">The list of projectiles this AverageAudio will use to calculate average positions. </param>
        /// <param name="sound">The Sound this AverageAudio will play. </param>
        public void SetProjectilesAndSound(List<Projectile> projectiles, Sound sound)
        {
            this.projectiles = projectiles;
            this.averageSound = sound;
        }
    }
    
}