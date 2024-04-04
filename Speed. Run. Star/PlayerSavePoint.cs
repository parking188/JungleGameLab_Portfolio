using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSavePoint : MonoBehaviour
{
    public Vector3 playerSavePoint;
    
    void TeleportToSavePoint()   
    {
        transform.position = playerSavePoint;
    }

    void OnTriggerEnter2D(Collider2D o)
    {
        if (o.gameObject.CompareTag("Cliff"))
        {
            TeleportToSavePoint();
        }
    }
}
