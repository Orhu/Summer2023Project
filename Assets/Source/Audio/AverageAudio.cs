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
        public AudioClip averageAudioClip;


        /// <summary>
        /// Adding an audio source and playing it after adjusting parameters
        /// </summary>
        void Start()
        {

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = averageAudioClip;
            audioSource.loop = true;
            //audioSource.playOnAwake = true; 
            audioSource.spatialBlend = 1f;
            audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
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
        /// Update the average pos of the projectiles. 
        /// </summary>
        // Update is called once per frame
        private void FixedUpdate()
        {
            transform.position = TryGetAveragePos();

        }

        /// <summary>
        /// Set the list of projectiles
        /// </summary>
        /// <param name="setProjectiles">The list of projectiles to set</param>
        public void SetProjectiles(List<Projectile> setProjectiles)
        {
            this.projectiles = setProjectiles;
        }
        /// <summary>
        /// Set the audioclip to play
        /// </summary>
        /// <param name="audioClip">AudioClip to be played</param>
        public void SetAudioClip(AudioClip audioClip)
        {
            this.averageAudioClip = audioClip;
        }
    }
    
}