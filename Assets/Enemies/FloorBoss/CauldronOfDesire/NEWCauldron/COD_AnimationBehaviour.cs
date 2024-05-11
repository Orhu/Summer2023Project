using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//keeps track of the number of cards the cauldron draws and resets after 3 have been flipped
public class COD_AnimationBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int picked = animator.GetInteger("cardsPicked");
        if (picked < 2)
        {
            animator.SetInteger("cardsPicked", picked += 1);
            return;
        }


        //already flipped! done attacking. If cardspicked == 0, then stop attack state.
        animator.SetInteger("cardsPicked", 0);
    }
}
