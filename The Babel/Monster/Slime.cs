using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IEnemy
{
    [Header("Movement")]
    public float jumpPower = 1000.0f;
    public float jumpPeriod = 2.0f;

    [Header("Attack")]
    public float knockbackForce;

    private PlayerController player; // 플레이어의 위치를 추적하기 위한 참조
    private Rigidbody2D rbSlime;
    private float timeAfterJump = 0.0f;
    private bool isCollisionWithPlatform;
    private RaycastHit2D rayHit;

    // Start is called before the first frame update
    void Start()
    {
        rbSlime = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Behave();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(player == null)
            {
                player = GameManager.Instance.player;
            }

            player.KnockBack(collision.contacts[0].normal * -1.0f, knockbackForce);
            Attack(player);
        }
        else if(collision.gameObject.CompareTag("Platform"))
        {
            isCollisionWithPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isCollisionWithPlatform = false;
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }

    public void Behave()
    {
        if(timeAfterJump < jumpPeriod)
        {
            timeAfterJump += Time.deltaTime;
        }
        else
        {
            
            rayHit = Physics2D.Raycast(rbSlime.position, Vector2.down, 3.0f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null && isCollisionWithPlatform)
            {
                timeAfterJump -= jumpPeriod;
                rbSlime.velocity *= new Vector3(1.0f, 0.0f, 1.0f);
                rbSlime.AddForce(new Vector2(0.0f, jumpPower), ForceMode2D.Impulse);
            }
        }
    }
}
