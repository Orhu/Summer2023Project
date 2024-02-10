using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//applied to projectile that should spin + change spin direction when it hits another surface
public class ProjectileRotation : MonoBehaviour
{
    private GameObject objToRotate;
    private Vector3 rollTransform = new Vector3(0.0f, 0.0f, 3f);
    private Transform startingParent;

    // Start is called before the first frame update
    void Start()
    {
        objToRotate = this.gameObject;
        startingParent = objToRotate.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //z increases = counter clockwise
        objToRotate.transform.Rotate(rollTransform);
        
        //check if we should play despawn animation
        if(transform.parent != startingParent)
        {
            GetComponent<Animator>().SetTrigger("despawn");
        }
    }

    public void SwitchRollDirection()
    {
        rollTransform = new Vector3(rollTransform.x, rollTransform.y, (rollTransform.z * -1f));
    }
}
