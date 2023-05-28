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

    /// <summary>
    /// Issue an attack from the given agent
    /// </summary>
    /// <param name="agent"> Agent who will issue the attack </param>
   public void PerformAttack(IActor agent)
    {
        Card cardToPlay = mainAttack;
        cardToPlay.PlayActions(agent);
    }
}
