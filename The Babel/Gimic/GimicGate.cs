using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicGate : MonoBehaviour
{
    public GameObject linkedGate;
    GameObject player;
    private void Start()
    {
        player = GameManager.Instance.player.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("게이트");
            player.transform.position = new Vector2(linkedGate.transform.position.x, linkedGate.transform.position.y + 2);
        }
    }
}
