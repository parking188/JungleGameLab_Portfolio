using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // Spring
    public float springHeigtBlock;
    public Vector2 springGoalPositionBlock;
    public float springUpTime;
    public float springDownTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.SetSpringOn(springHeigtBlock, springGoalPositionBlock, springUpTime, springDownTime);
        }
    }
}
