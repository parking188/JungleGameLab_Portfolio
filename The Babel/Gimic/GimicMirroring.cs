using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicMirroring : MonoBehaviour
{
    public GameObject origin;
    GameObject alterEgo;
    GameObject player;

    public GameObject openDoor;
    public GameObject startButton;
    public GameObject playerEndButton;
    public GameObject alterEgoEndButton;
    public GameObject goArrow;

    public bool isAlterClear = false;
    public bool isPlayerClear = false;

    Coroutine coroutine;

    private void Start()
    {
        player = GameManager.Instance.player.gameObject;
    }
    private void Update()
    {
        if(alterEgo != null)
        {
            player.GetComponent<PlayerController>().isSliding = alterEgo.GetComponent<PlayerController>().isSliding;
        }
    }

    public void StartGimic()
    {
        startButton.SetActive(false);
        playerEndButton.SetActive(true);
        CreateAlterEgo();
        coroutine = StartCoroutine(startingGimic());
    }

    public void CreateAlterEgo()
    {
        alterEgo = Instantiate(origin);
        alterEgo.transform.position = new Vector2(55, 70);
    }
    private void Reset()
    {
        Debug.Log("½ÇÆÐ");
        StopCoroutine(coroutine);
        Destroy(alterEgo);
        playerEndButton.SetActive(false);
        startButton.SetActive(true);
    }
    void ClearFloor()
    {
        Destroy(openDoor);
        Destroy(alterEgo);
        playerEndButton.SetActive(false);
        alterEgoEndButton.SetActive(false);
    }

    IEnumerator startingGimic()
    {
        int sec = 0;
        while(sec < 15)
        {
            yield return new WaitForSeconds(1f);
            sec++;

            if(isAlterClear)
            {
                goArrow.SetActive(true);
            }

            if (isPlayerClear && isAlterClear)
            {
                ClearFloor();
                yield break;
            }
        }
        Reset();
    }
}
