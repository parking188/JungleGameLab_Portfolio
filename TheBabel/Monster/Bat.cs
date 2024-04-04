using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, IEnemy
{
    [Header("Movement")]
    public float speed = 5f; //���ʹ� �ӵ�

    [Header("Attack")]
    public float knockbackForce;

    private PlayerController player; // �÷��̾��� ��ġ�� �����ϱ� ���� ����

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

        // �÷��̾ ���� �̵�
        transform.position = Vector3.MoveTowards(transform.position, player.gameObject.transform.position, speed * Time.deltaTime);
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }
}
