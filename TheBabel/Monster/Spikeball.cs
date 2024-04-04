using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikeball : MonoBehaviour, IEnemy
{
    [Header("Behave")]
    public Vector2[] movePoints;
    public float moveSpeed = 10.0f;
    public float rotateSpeed = 100.0f;

    [Header("Attack")]
    public float knockbackForce = 1000.0f;

    private int nextPointIndex = 0;

    // Update is called once per frame
    void Update()
    {
        Behave();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.Instance.player;
            player.KnockBack(collision.contacts[0].normal * -1.0f, knockbackForce);
            Attack(player);
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }

    public void Behave()
    {
        transform.Rotate(0.0f, 0.0f, rotateSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, movePoints[nextPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, movePoints[nextPointIndex]) < 0.001f)
        {
            nextPointIndex++;
            nextPointIndex %= movePoints.Length;
        }
    }
}