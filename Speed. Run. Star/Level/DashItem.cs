using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashItem : MonoBehaviour
{
    public float respawnTime = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().rigidData.isDashing = false;
            Invoke("RespawnDashItem", respawnTime);
            gameObject.SetActive(false);
        }
    }

    private void RespawnDashItem()
    {
        gameObject.SetActive(true);
    }
}
