using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles characters that must flip AFTER a flipping animation plays.
//add this behaviour to flipping animation state to flip the entire object on exit
public class FlipStateBehaviour : StateMachineBehaviour
{ 
    private GameObject Obj;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Obj = animator.gameObject;

        //flip on x axis
        Obj.transform.localScale = new Vector3((Obj.transform.localScale.x *-1), Obj.transform.localScale.y, Obj.transform.localScale.z);
    }
}