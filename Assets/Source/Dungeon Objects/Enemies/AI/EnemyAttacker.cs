using System.Collections;
using System.Collections.Generic;
using CardSystem;
using UnityEngine;

/// <summary>
/// Represents component to handle issuing enemy attacks
/// </summary>
public class EnemyAttacker : MonoBehaviour
{
    [Tooltip("Card representing the attack of this unit")]
    [SerializeField] private Card mainAttack; 
    
    [Tooltip("Number of attacks this unit will issue")]
    [SerializeField] private int attackCount;

    /// <summary>
    /// Issue an attack from the given agent
    /// </summary>
    /// <param name="agent"> Agent who will issue the attack </param>
   public void PerformAttack(IActor agent)
    {
        Card cardToPlay = mainAttack;
        int playCount = attackCount;
        List<ActionModifier> modifiers = cardToPlay.actionModifiers;

        cardToPlay.PlayActions(agent, playCount, modifiers);
    }
}
