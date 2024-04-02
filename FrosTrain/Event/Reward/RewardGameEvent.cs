using System;
using UnityEngine;

public class RewardGameEvent : IReward
{
    #region PublicVariables
    public SOEventData soEventData;

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
        InitRewardDelegate();
        isDispose = false;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => { if (soEventData != null) { GameEventManager.Instance.RegisterGameEvent(soEventData.Clone()); } };
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardGameEvent()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}