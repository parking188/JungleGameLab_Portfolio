using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicTriggerActive : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerController playerController;
    public Vector2 direction;
    public float distance = 15f;
    public float invokeTime = 0f;
    public float speed = 0.1f;
    public float knockbackPower = 3000f;

    public string target = "Player";    //레이어랑 태그 둘 다 한다....

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GameManager.Instance.player;
    }
    void Update()
    {
        //RaycastHit2D rayHit = Physics2D.Raycast(rb.position, direction, distance,LayerMask.GetMask("Player"));
        RaycastHit2D rayHit = Physics2D.Raycast(rb.position, direction, distance, LayerMask.GetMask(target));
        Debug.DrawRay(rb.position, direction * distance, Color.green);
        if (rayHit.collider != null && rayHit.collider.tag == target)
        {
            Invoke("DisableObject", invokeTime);
        }
        if(rayHit.collider)
            Debug.Log(rayHit.collider);
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
        Debug.Log("왜안돼는데!!!!!!!!");
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
            playerController.KnockBack(new Vector2(1, 1f), 3000f);
            playerController.Damage(1);
        }
    }
}
