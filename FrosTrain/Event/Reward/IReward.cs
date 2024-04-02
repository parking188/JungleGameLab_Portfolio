using System;
using UnityEngine;

public interface IReward
{
    #region PublicVariables
    public EActivateTiming _activateTiming { get; set; }
    public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public abstract void InitReward(IRewardTarget _rewardTarget = null);
    #endregion

    #region PrivateMethod
    #endregion
}

public enum EActivateTiming
{
    None,
    AddOrDeleteTrainCar,
    InitLaborVariables,
    MoveLaborerRandomCar,
    LaborSynergy,
    MoveLaborerRandomCarAfter,
    LaborSynergyAfter,
    Labor,
    LaborAfter,
    NewNode
}