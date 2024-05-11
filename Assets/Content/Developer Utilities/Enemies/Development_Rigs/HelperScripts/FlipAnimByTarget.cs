using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardificer
{
    //when all else fails, triggers animator by comparing its position to the player lol i got stuck
    //(used for slimes)
    public class FlipAnimByTarget : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        private bool flipped;
        private GameObject player;

        // Update is called once per frame
        void Update()
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                anim = GetComponentInChildren<Animator>();
                if (player == null || anim == null) { Destroy(this); }
            }

            if (player.gameObject.transform.position.x > 0 && flipped == false)
            {
                anim.SetTrigger("Flip");
                flipped = true;
            }

            if (player.gameObject.transform.position.x < 0 && flipped == true)
            {
                anim.SetTrigger("Flip");
                flipped = false;
            }
        }
    }

}