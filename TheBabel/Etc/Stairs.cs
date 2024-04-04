using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour
{
    public int nextSceneNumber;
    public string nextSpawnObjectName;

    private bool isPlayerIn = false;

    private void Update()
    {
        if (isPlayerIn && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.SceneStart(nextSceneNumber, nextSpawnObjectName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }
}
