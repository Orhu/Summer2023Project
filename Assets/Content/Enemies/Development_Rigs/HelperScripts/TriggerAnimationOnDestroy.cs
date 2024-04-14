using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    //trigger an animation when the object is destroyed
    public class TriggerAnimationOnDestroy : MonoBehaviour
    {
        [SerializeField] private string triggerName;
        private Animator anim;

        void Start()
        {

            if (GetComponent<Animator>() != null)
            {
                anim = gameObject.GetComponent<Animator>();
            }

            else if (gameObject.transform.GetComponent<Animator>() != null)
            {
                anim = gameObject.transform.GetComponent<Animator>();
            }

        }

        void OnDestroy()
        {
            anim.SetTrigger(triggerName);
            Debug.Log("triggered animation on Destroy");
        }
    }
}
