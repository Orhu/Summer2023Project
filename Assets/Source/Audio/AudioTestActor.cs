using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTestActor : MonoBehaviour, IActor
    {

        public SoundContainer _testSound;

        #region IActor stuff
        public Vector3 GetActionAimPosition()
        {
            throw new System.NotImplementedException();
        }

        public Transform GetActionSourceTransform()
        {
            throw new System.NotImplementedException();
        }

        public AudioSource GetAudioSource()
        {
            return transform.GetComponent<AudioSource>();
        }

        public Collider2D GetCollider()
        {
            throw new System.NotImplementedException();
        }

        public float GetDamageMultiplier()
        {
            throw new System.NotImplementedException();
        }

        public ref IActor.CanActRequest GetOnRequestCanAct()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.instance.PlayAudioAtPos(_testSound, new Vector2(0, 0));
                //AudioManager.instance.PlayAudioAtActor(_testSound, this);
            }
        }


    }

}
