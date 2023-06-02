using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // The damage dealt by this bomb.
    public DamageData damageData;

    // The radius in tiles of the explosion caused by the bomb.
    public float explosionRadius = 2f;

    // The time in seconds after the bomb is spawned until it detonates.
    public float fuseTime = 2f;

    // The objects not to damage when exploding.
    public List<GameObject> ignoredObjects = new List<GameObject>();


    // Invoked when this explodes.
    public System.Action onExploded;

    // Update is called once per frame
    private void Update()
    {
        fuseTime -= Time.deltaTime;
        if (fuseTime <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        onExploded?.Invoke();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (ignoredObjects.Contains(hitCollider.gameObject)) { continue; }

            hitCollider.GetComponent<Health>()?.ReceiveAttack(damageData);
        }

        transform.GetChild(0).transform.parent = null;
        Destroy(gameObject);
    }
}
