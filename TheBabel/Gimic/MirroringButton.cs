using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirroringButton : MonoBehaviour
{
    GimicMirroring gimicMirroring;
   
    private void Awake()
    {
        gimicMirroring = GetComponentInParent<GimicMirroring>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameObject.name == "StartButton")
        {
            gimicMirroring.StartGimic();
        }
        if (gameObject.name == "PlayerEndButton")
        {
            gimicMirroring.isPlayerClear = true;
        }
        if (gameObject.name == "AlterEgoEndButton")
        {
            gimicMirroring.isAlterClear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.name == "PlayerEndButton")
        {
            gimicMirroring.isPlayerClear = false;
        }
    }
}
