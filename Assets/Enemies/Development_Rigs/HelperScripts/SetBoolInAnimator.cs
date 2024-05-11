using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class SetBoolInAnimator : StateMachineBehaviour
    {
        //animator with bool to set
        public string objWithAnimatorName;
        public Animator subAnimator;
        public string boolName;
        public bool boolValue;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //TEMP: this is expensive
            if (subAnimator == null)
            {
                subAnimator = GameObject.Find(objWithAnimatorName).GetComponent<Animator>();
            }

            subAnimator.SetBool(boolName, boolValue);
        }
    }
}