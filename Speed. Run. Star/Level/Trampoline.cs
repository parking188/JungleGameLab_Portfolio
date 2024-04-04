
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    // trampoline
    
    public Vector2 trampolineDirection;
    public float trampolineTime;
    public float trampolineDistance;

    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.CompareTag("Player")){
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.SetTrampolineOn(trampolineDirection, trampolineTime, trampolineDistance * controller.rigidData.blockSize);
        }
    }
        
}
