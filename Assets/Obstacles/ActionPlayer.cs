using CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayer : MonoBehaviour, IActor
{
    public Action action;
    public float playFrequency = 1f;

    void Start()
    {
        StartCoroutine(PlayAction());
    }

    private IEnumerator PlayAction()
    {
        while(true)
        {
            yield return new WaitForSeconds(playFrequency);
            action.Play(this, 1, new List<ActionModifier>());
        }
    }

    public Transform GetActionSourceTransform()
    {
        return transform;
    }

    public Vector3 GetActionAimPosition()
    {
        return transform.position + transform.right;
    }

    public Collider2D GetCollider()
    {
        return null;
    }
}
