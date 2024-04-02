using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventButton : IGameEventButton
{
    #region PublicVariables
    public string context;
    [TextArea] public string popupContext;
    [SerializeReference] public List<ICondition> conditions = new List<ICondition>();
    [SerializeReference] public List<IReward> rewards = new List<IReward>();

    [Header("Text_Eng")]
    public string context_eng;
    [TextArea] public string popupContext_eng;
    #endregion

    #region PrivateVariables
    private bool isDispose;
    #endregion

    #region PublicMethod
    public void InitGameEventButton()
    {
        isDispose = false;
        InitConditionList();
        InitRewardList();
    }

    ~GameEventButton()
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
        //rewards.Clear();
        GC.SuppressFinalize(this);
    }

    public void GiveRewards()
    {
        if(rewards.Count > 0)
        {
            foreach (IReward reward in rewards)
            {
                reward.giveReward?.Invoke();
            }
        }
    }

    public bool CheckConditions()
    {
        if (conditions.Count <= 0)
        {
            return true;
        }

        bool isConditionsMet = true;
        foreach (ICondition condition in conditions)
        {
            condition.CheckCondition();
            isConditionsMet = isConditionsMet && condition.isConditionMet;

            if (isConditionsMet == false)
            {
                break;
            }
        }

        return isConditionsMet;
    }
    #endregion

    #region PrivateMethod
    private void InitConditionList()
    {
        if (conditions.Count > 0)
        {
            foreach (ICondition condition in conditions)
            {
                condition.InitCondition();        
                //condition.actionWhenConditionMet += CheckAndOccur;
            }
        }

        // 조건이 맞을 시 UI에 나타내기
        //CheckAndOccur();
    }

    private void InitRewardList()
    {
        if (rewards.Count > 0)
        {
            foreach(IReward reward in rewards)
            {
                reward.InitReward();
            }
        }

    }
    #endregion
}
