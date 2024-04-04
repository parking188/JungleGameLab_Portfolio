using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAddForcer : MonoBehaviour
{

    public float force;
    public Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.rigidbody.AddForce(direction.normalized*force*1.0f);
        //collision.rigidbody.AddForce(collision.relativeVelocity.normalized*force*-1.0f);
        //Debug.Log(direction.normalized * force * -1.0f);
    }
}
