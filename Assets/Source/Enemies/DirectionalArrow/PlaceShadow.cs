using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//positions shadow
public class PlaceShadow : MonoBehaviour
{
    private Vector3 targetOffset; //how much is the shadow offset from parent when upright?

    public Transform fakeParent; //used if shadow will change relative position based on something other than the parent
    private bool usingFakeParent = false;

    private SpriteRenderer shadowSprite;

    // Start is called before the first frame update
    void Start()
    {
        shadowSprite = GetComponentInChildren<SpriteRenderer>();

        //save original position so we can restore it every frame
        targetOffset = transform.localPosition;

        //offset will be difference between positions
        if(fakeParent != null)
        {
            targetOffset =  transform.position - fakeParent.position;
            usingFakeParent = true;
        }

        Debug.Log("target offset: " + targetOffset);
    }

    void Update()
    {
        SetRotation();

        //if we're using a non-parent as our "parent" disable the shadow when the parent is inactive
        if(usingFakeParent)
        {
            if(fakeParent.gameObject.activeInHierarchy == false) { shadowSprite.enabled = false; }
            else {  shadowSprite.enabled = true; }
        }
       
    }

    //make sure the shadow lies flat on the ground
    void SetRotation()
    {
        transform.rotation = Quaternion.identity;
        transform.up = Vector3.up;

        if(usingFakeParent)
        {
            transform.position = fakeParent.position + targetOffset;
        }
        else
        {
            transform.position = transform.parent.position + targetOffset;
        }
       
    }
}
