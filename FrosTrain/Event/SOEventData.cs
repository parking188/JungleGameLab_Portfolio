using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Event/Event Data", fileName = "New Event Data")]
public class SOEventData : ScriptableObject, IDisposable
{
    #region PublicVariables
    [HideIf("isReward", true)]
    public Sprite sprite;
    [HideIf("isReward", true)]
    public string title;
    [HideIf("isReward", true)]
    [TextArea(3, 10)] public string context;

    [SerializeReference] public List<ICondition> conditions = new List<ICondition>();
    [HideIf("isReward", true)]
    [SerializeReference] public List<IGameEventButton> buttonDatas = new List<IGameEventButton>();
    public int eventCount = 1;
    public float eventCooltime = 1f;
    public float chanceWeight = 1f;
    [HideIf("isForced", true)]
    public float alarmExpireTime = 30f;
    public bool isForced = false;

    public bool isReward = false;
    [ShowIf("isReward", true)]
    [SerializeReference] public List<IReward> rewards = new List<IReward>();

    [Header("Text_Eng")]
    [HideIf("isReward", true)]
    public string title_eng;
    [HideIf("isReward", true)]
    [TextArea(3, 10)] public string context_eng;
    public Sprite icon_sprite;
    #endregion

    #region PrivateVariables
    private bool isDispose;
    #endregion

    #region PublicMethod
    ~SOEventData()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        isDispose = true;
        conditions.Clear();
        buttonDatas.Clear();
        GC.SuppressFinalize(this);
    }

    public SOEventData Clone()
    {
        return Instantiate(this);
    }
    #endregion

    #region PrivateMethod
    #endregion
}