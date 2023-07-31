using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [Tooltip("The scale applied to each outline")]
    [SerializeField] private float scale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        int outlineIndex = transform.parent.GetComponentsInChildren<Outline>().Length;
        scale *= outlineIndex;
        scale += 1;
        transform.localScale = new Vector3(scale, scale, scale);
        GetComponent<SpriteRenderer>().sortingOrder = -outlineIndex;
    }
}
