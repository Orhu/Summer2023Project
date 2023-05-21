using System.Collections;
using System.Collections.Generic;
using CardSystem;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private Card mainAttack; // Card representing the attack of this unit
    [SerializeField] private int attackCount; // Number of attacks this unit will perform

   public void PerformAttack(IActor agent)
    {
        Card cardToPlay = mainAttack;
        int playCount = attackCount;
        List<ActionModifier> modifiers = cardToPlay.actionModifiers;

        cardToPlay.PlayActions(agent, playCount, modifiers);
    }
}
