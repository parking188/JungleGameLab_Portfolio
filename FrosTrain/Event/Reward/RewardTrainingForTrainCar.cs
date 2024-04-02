using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RewardTrainingForTrainCar : IReward
{
    #region PublicVariables
    public ESynergyType synergyType;
    public float value;

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
        InitRewardTarget(_rewardTarget);
        InitRewardDelegate();
        isDispose = false;
        _value = value;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => { ((TrainCar)rewardTarget).carContentBase.AddTrainingWithCycle(_value * (GameManager.Instance.player.train.TotalTrainingMultiple)); };

        switch (synergyType)
        {
            case ESynergyType.None:
                break;
            case ESynergyType.EducationSynergy:
                ((TrainCar)rewardTarget).carContentBase.GiveEducationRewardsAction += giveReward;
                break;
            case ESynergyType.TrainPowerSynergy:
                ((TrainCar)rewardTarget).carContentBase.GiveTrainPowerRewardsAction += giveReward;
                break;
            case ESynergyType.RedFrostTemplarSynergy:
                ((TrainCar)rewardTarget).carContentBase.GiveRedFrostTemplarRewardsAction += giveReward;
                break;
        }
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        rewardTarget = _rewardTarget;
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardTrainingForTrainCar()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}