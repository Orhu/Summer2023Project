using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles characters that must flip AFTER a flipping animation plays.
//add this behaviour to flipping animation state to flip the entire object on exit
public class FlipStateBehaviour : StateMachineBehaviour
{
    //the entire prefab we want to flip
    private GameObject Obj;

    //the sprite we want to flip
    private GameObject SpriteObj;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SpriteObj = animator.gameObject;
        Obj = SpriteObj.transform.parent.gameObject;

        //"un-flip" sprites for duration of flipping animation
        SpriteObj.transform.localScale = new Vector3((SpriteObj.transform.localScale.x * -1), SpriteObj.transform.localScale.y, SpriteObj.transform.localScale.z);

        //flip parent prefab
        Obj.transform.localScale = new Vector3((Obj.transform.localScale.x * -1), Obj.transform.localScale.y, Obj.transform.localScale.z);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SpriteObj = animator.gameObject;

        //finish by "re-flipping" sprites 
        SpriteObj.transform.localScale = new Vector3((SpriteObj.transform.localScale.x * -1), SpriteObj.transform.localScale.y, SpriteObj.transform.localScale.z);
    }
}