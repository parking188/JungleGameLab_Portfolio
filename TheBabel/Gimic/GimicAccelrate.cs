using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicAccelrate : MonoBehaviour
{
    public float power = 10000f;
    public Vector2 direction;
    PlayerController playerController;

    Rigidbody2D rig;
    private void Start()
    {
        direction.Normalize();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerController = collision.collider.GetComponent<PlayerController>();
            rig = playerController.GetComponent<Rigidbody2D>();
            rig.velocity = Vector2.zero;
            playerController.isSliding = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.GetComponent<Rigidbody2D>().AddForce(direction*power*Time.deltaTime);
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Invoke("ExitSlideing", 2f);
        }
    }

    void ExitSlideing()
    {
        playerController.isSliding = false;
    }
}
