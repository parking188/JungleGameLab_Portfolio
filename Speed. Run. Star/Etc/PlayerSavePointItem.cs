using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavePointItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D o)
    {
        if(o.gameObject.CompareTag("Player")){
            o.GetComponent<PlayerSavePoint>().playerSavePoint = transform.position;
        }
    }
}
