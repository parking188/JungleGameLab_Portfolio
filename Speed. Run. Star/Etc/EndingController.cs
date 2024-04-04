using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingController : MonoBehaviour
{
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI excellentText;
    public TextMeshProUGUI legendText;

    public float goodTime;
    public float excellentTime;
    public float legendTime;
    public TimerTextUpdater timerTextUpdater;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Camera.main.GetComponent<EndingCameraController>().EndingCameraWork();
            timerTextUpdater.isTimerOn = false;
            float nowTimer = timerTextUpdater.timer;
            PrintEndingText(nowTimer);
        }
       
    }

    private void PrintEndingText(float finishTime)
    {
        if(finishTime>= goodTime)
        {
            goodText.gameObject.SetActive(true);
            return;
        }

        if (finishTime >= excellentTime)
        {
            excellentText.gameObject.SetActive(true);
            return;
        }
        else
        {
            legendText.gameObject.SetActive(true);
        }
    }

}
