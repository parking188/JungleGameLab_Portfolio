using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imitator : MonoBehaviour, IEnemy
{
    [Header("Movement")]
    public float detectionRange = 10.0f;

    [Header("Attack")]
    public float knockbackForce;

    private PlayerController player; // 플레이어의 위치를 추적하기 위한 참조
    private Rigidbody2D rbPlayer;
    private Rigidbody2D rbImitator;

    // Start is called before the first frame update
    void Start()
    {
        rbImitator = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Behave();
    }
        
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, detectionRange);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            player.KnockBack(collision.contacts[0].normal * -1.0f, knockbackForce);
            Attack(player);
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }

    // 약간 에러있음
    public void Behave()
    {
        if (player == null || Vector3.Distance(this.transform.position, player.transform.position) > detectionRange)
        {
            player = GameManager.Instance.player;
            rbPlayer = player.GetComponent<Rigidbody2D>();
            return;
        }

        Vector2 reverseVelocity = rbPlayer.velocity * new Vector2(-1.0f, 1.0f);
        rbImitator.velocity = reverseVelocity;
    }
}
