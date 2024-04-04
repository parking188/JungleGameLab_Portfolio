using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour, IEnemy
{
    public float speed = 5f; //���ʹ� �ӵ�
    private PlayerController player; // �÷��̾��� ��ġ�� �����ϱ� ���� ����

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    private void Update()
    {
        Behave();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))         // �÷��̾�� �浹�� ���
        {
            Attack(player);
        }
    }

    public void Behave()
    {
        if(player == null)
        {
            return;
        }

        // �÷��̾ ���� �̵�
        transform.position = Vector3.MoveTowards(transform.position, player.gameObject.transform.position, speed * Time.deltaTime);
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Kill();      // ���� ���� ȣ��
    }
}
