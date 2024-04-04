using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasketball : MonoBehaviour, IEnemy, IEnemyProjectile
{
    private Vector3 moveDir;
    private float fSpeed = 10.0f;
    private float fRange = 80.0f;
    private Rigidbody2D rbBasketball;
    public float knockbackForce = 1000.0f;

    public float speed { get { return fSpeed; } set { fSpeed = value; } }
    public float range { get { return fRange; } set { fRange = value; } }

    void Start()
    {
        moveDir.Normalize();
        Behave();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Vector2 knockBackDirection = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
            playerController.KnockBack(knockBackDirection, knockbackForce);
            Attack(playerController);
            Destroy(this.gameObject);
        }
        else if (collision.CompareTag("Danger"))
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMoveDir(Vector3 moveDirection)
    {
        moveDir = moveDirection;
        float fAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.rotation.eulerAngles.z + fAngle - 90.0f);
    }

    public void Behave()
    {
        rbBasketball = GetComponent<Rigidbody2D>();
        rbBasketball.AddForce(moveDir * fSpeed, ForceMode2D.Impulse);
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }
}