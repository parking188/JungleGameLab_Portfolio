using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //----------------------------------------------------
    private static GameManager _instance;
    public static GameManager Instance
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

    public PlayerController player;
    public new CameraController camera;
    public TimerTextUpdater timerTextUpdater;

    public int star;

    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        star = 0;

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        if (camera == null)
        {
            camera = Camera.main.GetComponent<CameraController>();
        }
        camera.InitCamera();

        if (timerTextUpdater == null)
        {
            timerTextUpdater = GameObject.Find("TimerUI").GetComponent<TimerTextUpdater>();
        }
    }

    public void GameStart()
    {
        timerTextUpdater.isTimerOn = true;
    }

    public void StartButton()
    {
        SceneManager.LoadSceneAsync("TestScene_Eunseo").completed += oper =>
        {
            InitGame();
            GameStart();
        };
    }

    public void TutorialButton()
    {
        SceneManager.LoadSceneAsync("TutorialScene").completed += oper =>
        {
            InitGame();
            GameStart();
        };
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
