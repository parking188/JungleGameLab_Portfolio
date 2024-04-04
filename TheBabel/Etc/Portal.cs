using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string nextSpawnObjectName;

    private bool isPlayerIn = false;

    private void Update()
    {
        if (isPlayerIn && Input.GetKeyDown(KeyCode.F))
        {
            int randomSceneNumber = Random.Range(2, SceneManager.sceneCountInBuildSettings - 1);
            while(randomSceneNumber == SceneManager.GetActiveScene().buildIndex)
            {
                randomSceneNumber = Random.Range(2, SceneManager.sceneCountInBuildSettings - 1);
            }
            GameManager.Instance.SceneStart(randomSceneNumber, nextSpawnObjectName);
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
