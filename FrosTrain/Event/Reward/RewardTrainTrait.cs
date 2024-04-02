using System;
using UnityEngine;

public class RewardTrainTrait : IReward
{
    #region PublicVariables
    public SOTrainTrait soTrainTrait;

    public EActivateTiming _activateTiming { get; set; }
    public float _value { get; set; }
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
        InitRewardTarget();
        InitRewardDelegate();
        isDispose = false;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => { GameManager.Instance.player.train.AddTrainTraits(soTrainTrait); };
        cancelReward = () => { GameManager.Instance.player.train.RemoveTrainTraits(soTrainTrait); };
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        rewardTarget = _rewardTarget;
    }

    public void Dispose()
    {
        isDispose = true;
        //cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardTrainTrait()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}