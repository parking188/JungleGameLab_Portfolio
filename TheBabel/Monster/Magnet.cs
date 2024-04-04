using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour, IEnemy
{
    [Header("Behave")]
    public float magnetPower = 10.0f;
    private float detectionRange;

    [Header("Attack")]
    public float knockbackForce = 1000.0f;

    private PlayerController player;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        detectionRange = circleCollider.radius;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Behave();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
        if (player == null)
        {
            player = GameManager.Instance.player;
        }

        if (player.GetIsInvincible()
            || Vector3.Distance(this.transform.position, player.transform.position) > detectionRange)
        {
            return;
        }

        // 가까워질수록 약하게 당기기
        //player.transform.position = Vector3.Lerp(player.transform.position, this.transform.position, Time.deltaTime * magnetPower);

        // 가까워질수록 세게 당기기
        float playerToRangeDistance = (detectionRange - Vector3.Distance(player.transform.position, this.transform.position)) / detectionRange;
        player.transform.position = Vector3.MoveTowards(player.transform.position, this.transform.position, playerToRangeDistance * magnetPower * Time.deltaTime);
    }
}
