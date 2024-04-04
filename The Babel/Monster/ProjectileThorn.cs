using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThorn : MonoBehaviour, IEnemy, IEnemyProjectile
{
    private Vector3 moveDir;
    private float fSpeed = 10.0f;
    private float fRange = 80.0f;
    private float fMoveDistance;
    public float knockbackForce = 1000.0f;

    public float speed { get { return fSpeed; } set { fSpeed = value; } }
    public float range { get { return fRange; } set { fRange = value; } }

    void Start()
    {
        fMoveDistance = 0.0f;
        moveDir.Normalize();
    }


    void Update()
    {
        if(!GameManager.Instance.bIsGameActive)
        {
            return;
        }

        Behave();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Vector2 knockBackDirection = collision.ClosestPoint(transform.position) - (Vector2)transform.position;
            playerController.KnockBack(knockBackDirection, knockbackForce);
            Attack(playerController);
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("Platform"))
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
        float fMove = fSpeed * Time.deltaTime;
        transform.position += moveDir * fMove;
        fMoveDistance += fMove;

        if (fMoveDistance > fRange)
        {
            Destroy(gameObject);
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }
}
