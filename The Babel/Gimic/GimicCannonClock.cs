using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicCannonClock : MonoBehaviour
{
    GimicCannonNiddle cannonNiddle;
    private void Start()
    {
        cannonNiddle = GetComponentInChildren<GimicCannonNiddle>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
            cannonNiddle.ReadyCannon();
        }
    }
}
