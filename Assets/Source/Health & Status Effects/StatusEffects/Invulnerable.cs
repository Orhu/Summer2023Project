using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerable", menuName = "Status Effects [Don't Use]/Invulnerable")]
public class Invulnerable : StatusEffect
{
    internal override StatusEffect Instantiate(GameObject gameObject)
    {
        Invulnerable instance = (Invulnerable)base.Instantiate(gameObject);

        gameObject.GetComponent<Health>().onRequestIncomingAttackModification += instance.PreventAttack;

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

    void PreventAttack(ref Attack attack)
    {
        Attack prevousAttack = attack;
        attack = new Attack(0, prevousAttack.causer);
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.GetComponent<Health>().onRequestIncomingAttackModification -= PreventAttack;
    }
}
