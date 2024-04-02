using System;
using UnityEngine;
using UnityEngine.Events;

public class RewardGameObjectAction :IReward
{
    #region PublicVariables
    public GameObject targetGameObject;

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
        giveReward = () => { GameObject.Instantiate(targetGameObject); return; };
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardGameObjectAction()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}