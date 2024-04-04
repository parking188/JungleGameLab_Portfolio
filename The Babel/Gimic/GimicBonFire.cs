using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GimicBonFire : MonoBehaviour
{
    GameObject player;
    GimicSnowStorm gimicSnowStorm;

    private void Start()
    {
        player = GameManager.Instance.player.gameObject;
        gimicSnowStorm = GetComponentInParent<GimicSnowStorm>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(player))
        {
            Debug.Log("불 들어옴");
            gimicSnowStorm.StopCoroutine(gimicSnowStorm.downSpeed);
            gimicSnowStorm.upSpeed = gimicSnowStorm.StartCoroutine(gimicSnowStorm.UpSpeed());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(player))
        {
            Debug.Log("불 나감");
            gimicSnowStorm.StopCoroutine(gimicSnowStorm.upSpeed);
            gimicSnowStorm.downSpeed = gimicSnowStorm.StartCoroutine(gimicSnowStorm.DownSpeed());
        }
    }
}
