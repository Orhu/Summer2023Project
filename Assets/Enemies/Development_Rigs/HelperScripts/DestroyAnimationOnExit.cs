using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    public class DestroyAnimationOnExit : StateMachineBehaviour
    {
        //destroy object after the animation plays
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject, stateInfo.length);
        }
    }

}