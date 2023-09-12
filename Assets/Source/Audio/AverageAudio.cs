using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A class to leverage average audio positions of multiple transforms. 
    /// </summary>
    public class AverageAudio : MonoBehaviour
    {

        [Tooltip("The list of transforms to track")]
        public List<Transform> listOfTransformsToTrack = new List<Transform>();

        public AudioSource audioSource;
        public BasicSound sound;

        /// <summary>
        /// Get the average transform of the transforms
        /// </summary>
        /// <returns>Returns the average position of the transforms</returns>
        public Vector2 TryGetAveragePos()
        {
            float totalX = 0;
            float totalY = 0;

            foreach (var transform in listOfTransformsToTrack) 
            {
                if (transform != null)
                {
                    totalX += transform.position.x;
                    totalY += transform.position.y;
                }
            }

            return new Vector2(totalX / listOfTransformsToTrack.Count, totalY / listOfTransformsToTrack.Count);
        }

        /// <summary>
        /// Set the position of the GameObject to the average position of the transforms. 
        /// </summary>
        private void FixedUpdate()
        {
            transform.position = TryGetAveragePos();
        }

        public void DestroyAverageAudio(float fadeDuration)
        {
            AudioManager.instance.FadeToDestroy(audioSource, audioSource.volume, fadeDuration, true);
        }

    }

}