using System.Collections.Generic;
using UnityEngine;

//positions shadow
public class PlaceShadow : MonoBehaviour
{
    public bool varyPositionWithVelocity = false; //if true, the shadow's offset will decrease when velocity decreases
    public bool varyOpacityWithVelocity = false; //if true, the shadow's opacity will increase when velocity decreases
    public float velocityStartEffect = 2f; //what is slow enough to impact the shadow?
    public Vector3 minimumOffset = Vector3.zero; //minimum offset if it is changing with velocity
    public float maximumOpacity = .7f; //maximum opacity (when velocity is 0)
        
    public UnityEngine.Transform fakeParent; //used if shadow will change relative position based on something other than the parent
    
    private Vector3 targetOffset; //how much is the shadow offset from parent when upright?
    private bool usingFakeParent = false;
    private SpriteRenderer shadowSprite;

    private Vector3 prevPosition; //position of parent in last frame
    private List<float> recentVelocities = new List<float>();
    private float velocity = -1.0f;

    // Start is called before the first frame update
    void Start()
    {
        shadowSprite = GetComponentInChildren<SpriteRenderer>();

        //save original position so we can restore it every frame
        targetOffset = transform.localPosition;
        prevPosition = transform.parent.position;

        //offset will be difference between positions
        if (fakeParent != null)
        {
            targetOffset =  transform.position - fakeParent.position;
            prevPosition =  fakeParent.position;
            usingFakeParent = true;
        }
    }

    private void FixedUpdate()
    {
        //approximate velocity using last frame
        velocity = Mathf.Abs(Vector3.Distance(transform.parent.position, prevPosition)) / Time.deltaTime;
        if (usingFakeParent) { velocity = Mathf.Abs(Vector3.Distance(fakeParent.transform.position, prevPosition)) / Time.deltaTime; }

        //record this velocity
        if(recentVelocities.Count < 5) recentVelocities.Add(velocity);

        //list is full, shift to fit
        else
        {
            float[] tempVs = recentVelocities.ToArray();
            recentVelocities.Clear();

            for (int i = 1; i < tempVs.Length; i++) { recentVelocities.Add(tempVs[i]); }
            recentVelocities.Add(velocity);
        }

        //we want this to be smooth: use the average of up to the last 5 velocities
        float velocityNumerator = 0f;
        foreach (var v in recentVelocities) { velocityNumerator += v; }
        velocity = velocityNumerator / recentVelocities.Count;

        //record previous position of parent for next frame
        prevPosition = transform.parent.position;
        if (usingFakeParent) { prevPosition = fakeParent.transform.position; }
    }

    void Update()
    {
        SetRotation();

        //if we're using a non-parent as our "parent" disable the shadow when the parent is inactive
        if(usingFakeParent)
        {
            if(fakeParent.gameObject.activeInHierarchy == false) { shadowSprite.enabled = false; return; }
            else {  shadowSprite.enabled = true; }
        }


        //use velocity to manipulate shadow if allowed
        if ((varyOpacityWithVelocity || varyPositionWithVelocity) && velocity != -1.0f) 
        {
            if (varyOpacityWithVelocity) SetOpacity(velocity);
            if (varyPositionWithVelocity) SetLocalPosition(velocity);
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

    //higher velocity means the shadow will be more transparent
    void SetOpacity(float v)
    {
        //change color based on velocity
        float step = v / velocityStartEffect;
        float newAlpha = Mathf.Lerp(maximumOpacity, shadowSprite.color.a, step);

        shadowSprite.color = new Color(shadowSprite.color.r, shadowSprite.color.g, shadowSprite.color.b, newAlpha);
    }

    //sets position relative to parent: use if shadow should get closer to parent as velocity slows down
    void SetLocalPosition(float v)
    {
        string nameOfShadow = transform.parent.gameObject.name;
        if (usingFakeParent) nameOfShadow = fakeParent.gameObject.name;
        Debug.Log("Approx velocity of: " + nameOfShadow + " is: " + v);


        //change position based on velocity
        float step = v / velocityStartEffect;
        Vector3 newOffset = Vector3.Lerp(minimumOffset, targetOffset, step);

        if (usingFakeParent)
        {
            transform.position = fakeParent.position + newOffset;
        }
        else
        {
            transform.position = transform.parent.position + newOffset;
        }
    }
}
