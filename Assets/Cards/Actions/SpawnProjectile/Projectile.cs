using CardSystem.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // The spawner of the projectile.
    internal SpawnProjectile spawner;
    // The player of the projectile.
    internal ICardPlayer player;

    private Rigidbody2D rigidbody;
    private float distanceTraveled;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.radius = spawner.size;
        Physics2D.IgnoreCollision(collider, player.GetCollider());
        GetComponent<SpriteRenderer>().sprite = spawner.sprite;

        Vector3 diff = player.GetActionAimPosition() - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    private void FixedUpdate()
    {
        distanceTraveled += Time.fixedDeltaTime * spawner.speed;
        rigidbody.MovePosition(transform.position + transform.right * Time.fixedDeltaTime * spawner.speed);
        if (distanceTraveled > spawner.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Health health = collision.gameObject.GetComponent<Health>();
        //if (health != null)
        //{
        //    health.damage(spawner.damage);
        //}
        Destroy(gameObject);
    }
}
