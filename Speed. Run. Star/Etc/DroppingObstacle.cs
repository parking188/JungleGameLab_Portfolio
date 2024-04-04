using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DroppingObstacle : MonoBehaviour
{
    public GameObject Obstacle;
    public Vector3 initPosition;
    public float targetPositionY;
    public float droppingSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obstacle.transform.DOMoveY(targetPositionY, droppingSpeed);
        }
    }

    public void OnReset()
    {
        Obstacle.transform.localPosition = initPosition;
    }
}
