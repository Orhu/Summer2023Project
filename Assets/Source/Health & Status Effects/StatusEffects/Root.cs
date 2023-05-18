using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : StatusEffect
{
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Root instance = CreateInstance<Root>();

        instance.Duration = Duration;
        instance.gameObject = gameObject;

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

    private void OnDestroy()
    {
        gameObject.GetComponent<Movement>().requestSpeedModifications -= PreventMovement;
    }
}
