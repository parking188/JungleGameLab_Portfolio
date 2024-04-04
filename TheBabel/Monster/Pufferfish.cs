using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pufferfish : MonoBehaviour, IEnemy
{
    [Header("Behave")]
    public float detectionRange = 5.0f;
    public SpriteRenderer bodySprite;
    public Color defaultColor = Color.white;
    public Color blowColor = Color.yellow;
    public float maxScale = 4.0f;
    public float minScale = 2.5f;
    public float blowSpeed = 0.8f;

    [Header("Attack")]
    public float knockbackForce;

    private PlayerController player; // 플레이어의 위치를 추적하기 위한 참조

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.player;
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
        if (player == null || Vector3.Distance(this.transform.position, player.transform.position) > detectionRange)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * minScale, blowSpeed * Time.deltaTime);
            bodySprite.color = Color.Lerp(bodySprite.color, defaultColor, blowSpeed * Time.deltaTime);
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxScale, blowSpeed * Time.deltaTime);
        bodySprite.color = Color.Lerp(bodySprite.color, blowColor, blowSpeed * Time.deltaTime);
    }
}