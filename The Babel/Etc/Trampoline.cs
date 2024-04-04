using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float powerMultiplied = 2.5f;
    public Vector2 direction = Vector2.up;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        //if (rb && rb.velocity.y < 0)
        //{
        //    Debug.Log("ºÎµúÈù³ð: " + rb.velocity.y + collision.gameObject.name);
        //    Vector2 addSpeed = direction.normalized * -rb.velocity.y;
        //    Vector2 nv = new Vector2(rb.velocity.x + addSpeed.x, addSpeed.y * powerMultiplied);
        //    rb.velocity = nv;
        //    Debug.Log("²ó" + rb.velocity + nv);
        //}

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb && collision.relativeVelocity.y < 0)
        {
            Vector2 addSpeed = direction.normalized * -collision.relativeVelocity.y;
            Vector2 nv = new Vector2(rb.velocity.x + addSpeed.x, addSpeed.y * powerMultiplied);
            Debug.Log(nv);
            rb.velocity = nv;
        }
    }

}
