using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A class to leverage average audio positions of multiple transforms. 
    /// </summary>
    public class AverageAudio : MonoBehaviour
    {
        [Tooltip("The list of transforms to track")]
        public List<Transform> transforms = new List<Transform>();

        [Tooltip("The Sound to play at the averaged location")] 
        public BasicSound averageSound;


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
        /// Get the average transform of the transforms
        /// </summary>
        /// <returns>Returns the average position of the transforms</returns>
        public Vector2 TryGetAveragePos()
        {
            float totalX = 0;
            float totalY = 0;

            foreach (var transform in transforms)
            {
                if (transform != null)
                {
                    totalX += transform.position.x;
                    totalY += transform.position.y;
                }
            }

            if (totalX == 0 && totalY == 0)
            {
                AudioManager.KillAverageAudio(this);
            }

            return new Vector2(totalX / transforms.Count, totalY / transforms.Count);
        }

        /// <summary>
        /// Set the position of the GameObject to the average position of the transforms. 
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
        /// Sets the list of Transforms and the Sound used for this AverageAudio. 
        /// </summary>
        /// <param name="transforms">The list of projectiles this AverageAudio will use to calculate average positions. </param>
        /// <param name="sound">The Sound this AverageAudio will play. </param>
        public void SetTransformsAndSound(List<Transform> transforms, BasicSound sound)
        {
            this.transforms = transforms;
            this.averageSound = sound;
        }
    }

}