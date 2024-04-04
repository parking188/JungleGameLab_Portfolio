using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
    void Start()
    {
        
    }
    public void SceneStartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void RestartButton()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }
}