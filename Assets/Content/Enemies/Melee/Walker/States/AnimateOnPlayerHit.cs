using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//applied to a gameObject with an animator to trigger an attack animaton when the player is detected.
public class AnimateOnPlayerHit : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if(anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    //play animation if we run into the player
    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            anim.SetTrigger("attack");
        }
    }
}
