using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicSnowStorm : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    public Coroutine downSpeed;
    public Coroutine upSpeed;
    void Awake()
    {
        playerController = GameManager.Instance.player;
        player = playerController.gameObject;
    }
    void Start()
    {
        downSpeed = StartCoroutine(DownSpeed());
    }
    public IEnumerator DownSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            playerController.fSpeed -= 0.5f;
            GameManager.Instance.AddWhiteBalanceTemperature(-4.0f, -100.0f, 0.0f);
            if (playerController.fSpeed <= 2)
            {
                playerController.Damage(1);
            }
            if (playerController.fSpeed <= 0)
            {
                playerController.fSpeed = 0f;
                yield break;
            }
        }
    }
    public IEnumerator UpSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            playerController.fSpeed += 1.5f;
            GameManager.Instance.AddWhiteBalanceTemperature(12.0f, -100.0f, 0.0f);
            if (playerController.fSpeed >= 15)
            {
                playerController.fSpeed = 15;
                yield break;
            }
        }
    }
}
