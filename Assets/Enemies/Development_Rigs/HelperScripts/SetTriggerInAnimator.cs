using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class SetTriggerInAnimator : StateMachineBehaviour
    {
        //animator with trigger to set
        public string objWithAnimatorName;
        public Animator subAnimator;
        public string trigger;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //TEMP: this is expensive
            if (subAnimator == null)
            {
                subAnimator = GameObject.Find(objWithAnimatorName).GetComponent<Animator>();
            }

            subAnimator.SetTrigger(trigger);
        }
    }
}