using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicBreakable : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerController playerController;
    public Vector2 direction;
    public float invokeTime = 0f;
    public float speed = 0.1f;
    public float knockbackPower = 3000f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(rb.position,direction, 15f,LayerMask.GetMask("Player"));
        Debug.DrawRay(rb.position, direction*15f, Color.green);
        if (rayHit.collider != null && rayHit.collider.tag == "Player")
        {
            Invoke("ForceObject", invokeTime);
        }
    }

    void ForceObject()
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "BreakablePlatform")
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 데미지");
            playerController = GameManager.Instance.player;
            playerController.KnockBack(new Vector2(1,1f),3000f);
            playerController.Damage(1);
        }
    }
}
