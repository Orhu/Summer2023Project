using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Silence", menuName = "Status Effects [Don't Create]/Silence")]
public class Silence : StatusEffect
{
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Silence instance = (Silence)base.Instantiate(gameObject);

        gameObject.GetComponent<Controller>().GetOnRequestCanAct() += instance.PreventAction;

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

    private void PreventAction(ref bool CanAct)
    {
        CanAct = false;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Controller>().GetOnRequestCanAct() -= PreventAction;
    }
}
