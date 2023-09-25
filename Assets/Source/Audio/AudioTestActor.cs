using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{

    /// <summary>
    /// Used for testing audio
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioTestActor : MonoBehaviour, IActor
    {

        [Tooltip("Sound to test")]
        public SoundContainer _testSound;

        [Tooltip("AudioClip to test")]
        public AudioClip _testClip;

        #region IActor stuff
        /// <summary>
        /// Not used for testing audio
        /// </summary>
        public Vector3 GetActionAimPosition()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not used for testing audio
        /// </summary>
        public Transform GetActionSourceTransform()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get the AudioSource on this IActor
        /// </summary>
        public AudioSource GetAudioSource()
        {
            return transform.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Not used for testing audio
        /// </summary>
        public Collider2D GetCollider()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not used for testing audio
        /// </summary>
        public float GetDamageMultiplier()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Not used for testing audio
        /// </summary>
        public ref IActor.CanActRequest GetOnRequestCanAct()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Used for playing audio
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //AudioManager.instance.PlaySoundAtPos(_testSound, new Vector2(0, 0));
                //AudioManager.instance.PlayAudioAtActor(_testSound, this);
            }
        }


    }

}
