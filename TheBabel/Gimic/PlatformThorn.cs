using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformThorn : MonoBehaviour
{
    public float knockbackForce = 1000.0f;

    private PlayerController player;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(player == null)
            {
                player = GameManager.Instance.player;
            }

            Vector3 knockbackDirection = player.transform.position - transform.position;
            player.KnockBack(knockbackDirection, knockbackForce);
            player.Damage(1);
        }
    }
}
