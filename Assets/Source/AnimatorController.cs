using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    [Tooltip("The states to the parameter name to use to mirror")]
    [SerializeField] private 

    // The animator to control
    //private Animator animator;

    /// <summary>
    /// Initialize reference.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets the value of the given boolean parameter.
    /// </summary>
    /// <param name="name"> The parameter name. </param>
    /// <param name="value"> The new parameter value. </param>
    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    /// <summary>
    /// Sets the value of the given trigger parameter.
    /// </summary>
    /// <param name="name"> The parameter name. </param>
    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }
}
