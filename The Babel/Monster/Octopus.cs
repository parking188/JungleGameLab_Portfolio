using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour, IEnemy
{
    public float speed = 5f; //에너미 속도
    private PlayerController player; // 플레이어의 위치를 추적하기 위한 참조

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
        if (collision.CompareTag("Player"))         // 플레이어와 충돌한 경우
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

        // 플레이어를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, player.gameObject.transform.position, speed * Time.deltaTime);
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Kill();      // 게임 오버 호출
    }
}
