using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpsideDown : MonoBehaviour
{
    private CameraController cameraController;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(cameraController.UpsideDown());
        }
    }
}
