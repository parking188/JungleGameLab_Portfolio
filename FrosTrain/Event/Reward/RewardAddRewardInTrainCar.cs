using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardAddRewardInTrainCar : IReward
{
    #region PublicVariables
    public bool isPermanent = false;
    public bool isTargetSelf = false;
    [SerializeReference] public List<IReward> rewards;

    public EActivateTiming _activateTiming { get; set; }
    [HideInInspector] public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    private bool isDispose;
    #endregion

    #region PublicMethod
    public void InitReward(IRewardTarget _rewardTarget = null)
    {
        InitRewardTarget(_rewardTarget);
        InitRewardList(rewardTarget);
        InitRewardDelegate();
        isDispose = false;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => {
            TrainCar trainCar = (TrainCar)rewardTarget;
            trainCar.AddRewards(rewards);
        };

        cancelReward = () =>
        {
            if(isPermanent == false)
            {
                TrainCar trainCar = (TrainCar)rewardTarget;
                trainCar.RemoveRewards(rewards);
            }
        };
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        if(isTargetSelf && rewardTarget != null)
        {
            return;
        }

        rewardTarget = _rewardTarget;
    }

    private void InitRewardList(IRewardTarget _rewardTarget = null)
    {

        for(int index = 0; index < rewards.Count; index++)
        {
            rewards[index].InitReward(_rewardTarget);
        }
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardAddRewardInTrainCar()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}