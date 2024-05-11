using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//applied to projectile that should spin + change spin direction when it hits another surface
public class ProjectileRotation : MonoBehaviour
{
    //previous position - used to see when ball should change direction
    private Vector3 prevPos;

    private Vector3 rollTransform = new Vector3(0.0f, 0.0f, 3f);
    private Transform startingParent;

    //Start is called before the first frame update
    void Start()
    {
        prevPos = gameObject.transform.position;
        startingParent = gameObject.transform.parent;
    }

    void FixedUpdate()
    { 
        gameObject.transform.Rotate(rollTransform);
    }

    public void SwitchRollDirection()
    {
        //note: increasing z rolls counter clockwise and decreasing z rolls clockwise
        rollTransform = new Vector3(rollTransform.x, rollTransform.y, (rollTransform.z * -1f));
    }
}
