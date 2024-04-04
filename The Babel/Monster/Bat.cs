using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, IEnemy
{
    [Header("Movement")]
    public float speed = 5f; //에너미 속도

    [Header("Attack")]
    public float knockbackForce;

    private PlayerController player; // 플레이어의 위치를 추적하기 위한 참조

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    private void Update()
    {
        Behave();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            player.KnockBack(collision.contacts[0].normal * -1.0f, knockbackForce);
            Attack(player);
        }
    }

    public void Behave()
    {
        if (player == null)
        {
            return;
        }

        // 플레이어를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, player.gameObject.transform.position, speed * Time.deltaTime);
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }
}
