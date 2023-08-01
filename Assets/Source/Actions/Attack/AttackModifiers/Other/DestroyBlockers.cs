using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// Causes an attack to destroy blockers.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDestroyBlockers", menuName = "Cards/AttackModifers/Destroy Blockers")]
    public class DestroyBlockers : AttackModifier
    {
        // Binds Destroys blockers on hit
        public override void Initialize(Projectile value)
        {
            value.onHit +=
                (Collision2D collision) =>
                {
                    if (collision.gameObject.layer != LayerMask.NameToLayer("Blockers")) { return; }

                    if (--value.remainingHits > 0)
                    {
                        value.CancelInvoke("DestroyOnWallHit");
                    }
                    Destroy(collision.gameObject);
                };
        }
    }
}
