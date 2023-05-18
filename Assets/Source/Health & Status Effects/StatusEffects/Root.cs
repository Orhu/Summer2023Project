using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Root", menuName = "Status Effects [Don't Create]/Root")]
public class Root : StatusEffect
{
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Root instance = (Root)base.Instantiate(gameObject);

        gameObject.GetComponent<Movement>().requestSpeedModifications += instance.PreventMovement;

        return instance;
    }

    internal override bool Stack(StatusEffect other)
    {
        if (other.GetType() != GetType())
        {
            return false;
        }

        other.Duration += Duration;
        return true;
    }

    void PreventMovement(out float speed)
    {
        speed = 0;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Movement>().requestSpeedModifications -= PreventMovement;
    }
}
