using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningStone : MonoBehaviour, IEnemy
{
    public float knockbackForce = 1000.0f;

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }

    public void Behave()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Vector2 knockBackDirection = collision.contacts[0].point - (Vector2)transform.position;
            playerController.KnockBack(knockBackDirection, knockbackForce);
            Attack(playerController);
        }
    }
}
