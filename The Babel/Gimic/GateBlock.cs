using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBlock : MonoBehaviour
{
    GameObject player;
    public GameObject linkedGate;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    public void UseGate()
    {
        player.transform.position = new Vector2(linkedGate.transform.position.x, linkedGate.transform.position.y+2);
    }
}
