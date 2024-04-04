using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicFallingPlatform : MonoBehaviour
{
    Vector2 originPos;
    private void Start()
    {
        originPos = transform.position;
    }
    public void Falling()
    {
        Invoke("FallingPlat",1f);
    }

    void FallingPlat()
    {
        GetComponent<Rigidbody2D>().gravityScale = 5f;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void Reload()
    {
        transform.position = originPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null && !collision.collider.CompareTag("Player"))
        {
            transform.position = new Vector2(300, 300);
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Invoke("Reload", 7f);
        }
    }
}
