using CardSystem;
using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitActionPlayer : MonoBehaviour, IActor
{
    public void Play(Object causer, List<Action> actions)
    {
        List<GameObject> ignoredObjects = causer is Component ? new List<GameObject> { (causer as Component).gameObject } : null;
        foreach (Action action in actions)
        {
            if (action is Attack)
            {
                (action as Attack).Play(this, causer as GameObject, ignoredObjects);
            }
            else
            {
                action.Play(this, ignoredObjects);
            }
        }

        Invoke("DestroyThis", 120);
    }

    void DestroyThis()
    {
        Destroy(this);
    }

    #region IActor Implementation
    public Vector3 GetActionAimPosition()
    {
        return transform.position + transform.right;
    }

    public Transform GetActionSourceTransform()
    {
        return transform;
    }

    public Collider2D GetCollider()
    {
        return GetComponent<Collider2D>();
    }

    IActor.CanActRequest canAct;
    public ref IActor.CanActRequest GetOnRequestCanAct()
    {
        return ref canAct;
    }
    #endregion
}
