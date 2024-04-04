using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    public float maxDistance;
    public float moveSpeed;
    bool isTurn = false;
    public bool isRotate = false;
    float originPosition;
    void Start()
    {
        originPosition = transform.position.x;
        if (isRotate)
        {
            isTurn = !isTurn;
        }
    }

    void FixedUpdate()
    {
        if(isTurn)
        {
            transform.Translate(Vector2.left * moveSpeed);
        }
        else
        {
            transform.Translate(Vector2.right * moveSpeed);
        }
        if (Mathf.Abs(originPosition - transform.position.x) >= maxDistance)
        {
            originPosition = transform.position.x;
            isTurn = !isTurn;
        }
    }

    float distance;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            distance = transform.position.x - collision.gameObject.transform.position.x;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject player = collision.gameObject;
            Rigidbody2D playerRigid = player.GetComponent<Rigidbody2D>();
            if(playerRigid.velocity == new Vector2(0,0))
            {
                player.transform.position = new Vector2(transform.position.x - distance, player.transform.position.y);
            }
            else
            {
                distance = transform.position.x - collision.gameObject.transform.position.x;
            }
        }
    }
}
