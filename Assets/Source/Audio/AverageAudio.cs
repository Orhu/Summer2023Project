using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cardificer
{
    /// <summary>
    /// A class that keeps a GameObject at an average position between a list of Transforms. 
    /// </summary>
    public class AverageAudio : MonoBehaviour
    {

        [Tooltip("The list of transforms to track")]
        public List<Transform> listOfTransformsToTrack = new List<Transform>();

        [Tooltip("The AudioSource playing a BasicSound")]
        public AudioSource audioSource;

        [Tooltip("The BasicSound to play on this GameObject")]
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

        /// <summary>
        /// Fade out the BasicSound playing on this GameObject and destroy this GameObject once the AudioSource has reached 0 volume
        /// </summary>
        public void DestroyAverageAudio(float fadeDuration)
        {
            AudioManager.instance.FadeToDestroy(audioSource, audioSource.volume, fadeDuration, true);
        }

    }

}