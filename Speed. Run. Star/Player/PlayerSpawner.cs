using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    //----------------------------------------------------
    private static PlayerSpawner _instance;
    public static PlayerSpawner Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    //----------------------------------------------------

    public GameObject playerPrefab;

    public GameObject SpawnPlayer(Vector3 spawnPosition)
    {
        return Instantiate(playerPrefab, spawnPosition, playerPrefab.transform.rotation);
    }
}
