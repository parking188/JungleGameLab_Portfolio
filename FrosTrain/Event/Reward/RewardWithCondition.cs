using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardWithCondition : IReward
{
    #region PublicVariables
    public EActivateTiming activateTiming;
    [SerializeReference] public List<ICondition> conditions = new List<ICondition>();
    [SerializeReference] public List<IReward> rewards = new List<IReward>();

    public EActivateTiming _activateTiming { get; set; }
    [HideInInspector] public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    private bool isDispose;
    private bool isPacked;
    private bool isLabored;
    #endregion

    #region PublicMethod
    public void InitReward(IRewardTarget _rewardTarget = null)
    {
        rewardTarget = _rewardTarget;
        isPacked = false;
        isLabored = false;
        InitConditionList();
        InitRewardList(_rewardTarget);
        InitRewardDelegate();
        RegistActivateTiming();
        isDispose = false;
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        conditions.Clear();
        rewards.Clear();
        GC.SuppressFinalize(this);
    }

    ~RewardWithCondition()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () =>
        {
            bool packedCheck = isPacked ? !isLabored : true;

            if (packedCheck && CheckConditions())
            {
                for(int index = 0; index < rewards.Count; index++)
                {
                    rewards[index].giveReward?.Invoke();
                }
                isLabored = true;
            }
        };

        cancelReward = () =>
        {
            for(int index = 0; index < rewards.Count; index++)
            {
                rewards[index].cancelReward?.Invoke();
            }
        };

        afterCycle = () => { isLabored = false; };
    }

    private void InitConditionList()
    {
        for(int index = 0; index < conditions.Count; index++)
        {
            if (rewardTarget is IConditionTarget)
            {
                conditions[index].InitCondition((IConditionTarget)rewardTarget);
            }
            else
            {
                conditions[index].InitCondition();
            }

            if (conditions[index] is ConditionTrainCar)
            {
                if (((ConditionTrainCar)conditions[index]).condition == ConditionTrainCar.ETrainCarCondition.Packed)
                {
                    isPacked = true;
                }
            }
        }
    }

    private void InitRewardList(IRewardTarget _rewardTarget = null)
    {
        for(int index = 0; index < rewards.Count; index++)
        {
            rewards[index].InitReward(_rewardTarget);
        }
    }

    private bool CheckConditions()
    {
        for(int index = 0; index < conditions.Count; index++)
        {
            conditions[index].CheckCondition();
            if (conditions[index].isConditionMet == false)
            {
                return false;
            }
        }

        return true;
    }


    private void RegistActivateTiming()
    {
        _activateTiming = activateTiming;
        switch (_activateTiming)
        {
            case EActivateTiming.None:
                break;
            case EActivateTiming.AddOrDeleteTrainCar:
                GameManager.Instance.player.train.AddOrDeleteTrainCarAction += giveReward;
                break;
            case EActivateTiming.InitLaborVariables:
                GameManager.Instance.player.train.InitLaborVariablesAction += giveReward;
                break;
            case EActivateTiming.MoveLaborerRandomCar:
                GameManager.Instance.player.train.MoveLaborerRandomCarAction += giveReward;
                break;
            case EActivateTiming.LaborSynergy:
                GameManager.Instance.player.train.LaborSynergyAction += giveReward;
                break;
            case EActivateTiming.MoveLaborerRandomCarAfter:
                GameManager.Instance.player.train.MoveLaborerRandomCarAfterAction += giveReward;
                break;
            case EActivateTiming.LaborSynergyAfter:
                GameManager.Instance.player.train.LaborSynergyAfterAction += giveReward;
                break;
            case EActivateTiming.Labor:
                GameManager.Instance.player.train.LaborAction += giveReward;
                break;
            case EActivateTiming.LaborAfter:
                GameManager.Instance.player.train.LaborAfterAction += giveReward;
                break;
            case EActivateTiming.NewNode:
                ExploreManager.Instance.guiExploreMap.NewNodeAction += giveReward;
                break;
        }
    }
    #endregion
}