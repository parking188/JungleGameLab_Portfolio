using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerTextUpdater : MonoBehaviour
{
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI secondText;
    public TextMeshProUGUI miliSecondText;
    public bool isTimerOn;

    public float timer;
    private float minute;
    private float second;
    private float miliSecond;


    void UpdateTimerText()
    {
        if (isTimerOn)
        {
            timer += Time.deltaTime;
            minute = (int)timer / 60;
            second = ((int)timer - minute * 60) % 60;
            miliSecond = timer - Mathf.Floor(timer);


            string minuteString = minute.ToString().PadLeft(2, '0');
            string secondString = second.ToString().PadLeft(2, '0');
            minuteText.text = minuteString;
            secondText.text = secondString;
            miliSecondText.text = ((int)(miliSecond * 100)).ToString("D2");
        }
    }
    public void InitTimer()
    {
        timer = 0;
        miliSecond = 0;
        second = 0;
        minute = 0;
        minuteText.text = "00";
        secondText.text = "00";
        miliSecondText.text = "00";
    }

    private void Update()
    {
        UpdateTimerText();
    }
}
