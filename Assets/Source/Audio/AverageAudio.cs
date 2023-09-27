using System.Collections;
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

        //Checks whether or not we are already destroying the average audio
        private bool destroyingAverageAudio = false;

        private float minX, minY, maxX, maxY;

        private void Start()
        {

        }

        /// <summary>
        /// Get the average transform of the transforms
        /// </summary>
        /// <returns>Returns the average position of the transforms</returns>
        public Vector2 TryGetAveragePos()
        {

            if (listOfTransformsToTrack.Count <= 0)
            {
                if (!destroyingAverageAudio)
                {
                    DestroyAverageAudio(0.5f);
                }
                return transform.position;
            }
            else
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

        }

        /// <summary>
        /// Set the position of the GameObject to the average position of the transforms. 
        /// </summary>
        private void FixedUpdate()
        {
            transform.position = TryGetAveragePos();

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            if ((screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width) && !destroyingAverageAudio)
            {
                DestroyAverageAudio(1f);
            }
        }

        /// <summary>
        /// Fade out the BasicSound playing on this GameObject and destroy this GameObject once the AudioSource has reached 0 volume
        /// </summary>
        public void DestroyAverageAudio(float fadeDuration)
        {
            if (AudioManager.instance.printDebugMessages) print("destroying AverageAudio: " + gameObject.name + ". fadeDuration = " + fadeDuration);
            destroyingAverageAudio = true;
            AudioManager.instance.FadeToDestroy(audioSource, audioSource.volume, fadeDuration, true);
        }

    }

}