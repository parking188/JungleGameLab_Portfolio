using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();

            if (playerController.rigidData.canGetStar)
            {
                GameManager.Instance.star++;
                playerController.rigidData.canGetStar = false;
                Destroy(this.gameObject);
            }
        }
    }
}
