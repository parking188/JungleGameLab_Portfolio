using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLevelData : MonoBehaviour
{
    #region PublicVariables
    #endregion
    #region PrivateVariables
    public bool canSave;
    [SerializeField]private float exp;
    [SerializeField]private float maxExp;
    [SerializeField]private int level;
    [SerializeField]private int curTrainCar;
    [SerializeField]private int maxTrainCar;
    #endregion
    #region PublicMethod
    public void Init()
    {
        LoadLevel();
        LoadExp();
    }

    public void LoadExp()
    {
        exp = ES3.Load("exp", 0f);
        GameManager.Instance.player.train.AddExp(exp);
    }

    public void LoadLevel()
    {
        level = ES3.Load("level", 0);
        GameManager.Instance.player.train.LevelSet(level);
    }

    public void SaveExp(float _exp)
    {
        if(canSave == true)
        {
            exp = _exp;
            ES3.Save("exp", _exp);
        }
    }

    public void SaveLevel(int _level)
    {
        if (canSave == true)
        {
            level = _level;
            ES3.Save("level", _level);
        }
    }
    #endregion
    #region PrivateMethod
    #endregion
}