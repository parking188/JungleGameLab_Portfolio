using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicGiant : MonoBehaviour
{
    float giantScale = 5;
    public bool isgiant = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(isgiant)
            {
                collision.gameObject.transform.localScale = new Vector3(giantScale, giantScale, giantScale);
            }
            else
            {
                collision.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            Destroy(gameObject);
        }
    }
}
