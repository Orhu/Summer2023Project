using UnityEngine;

/// <summary>
/// Deals damage when objects overlap triggers on this object.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DamageOnOverlap : MonoBehaviour
{
    [Tooltip("The damage that will be dealt.")]
    [SerializeField] private DamageData damageData;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Health>()?.ReceiveAttack(damageData);
    }
}
