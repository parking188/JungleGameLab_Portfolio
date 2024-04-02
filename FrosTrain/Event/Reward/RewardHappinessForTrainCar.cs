using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RewardHappinessForTrainCar : IReward
{
    #region PublicVariables
    public ESynergyType synergyType;
    public bool isRandom = false;
    public bool isZeal = false;
    public bool isWithPermanentHappiness = false;
    [HideIf("isRandom", true)]
    public float value;
    [ShowIf("isRandom", true)]
    public int minValue;
    [ShowIf("isRandom", true)]
    public int maxValue;

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
        giveReward = () => {
            float curValue = _value;
            if (isRandom) curValue = UnityEngine.Random.Range(minValue, maxValue);
            if (isZeal) curValue = curValue * ResourceManager.Instance.GetZeal().Value;
            ((TrainCar)rewardTarget).carContentBase.AddHappinessWithCycle(Mathf.FloorToInt(curValue + (isWithPermanentHappiness ? ((TrainCar)rewardTarget).soTrainCar.permanentHappiness : 0)));
        };

        switch(synergyType)
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

    ~RewardHappinessForTrainCar()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion
}

public enum ESynergyType
{
    None,
    EducationSynergy,
    TrainPowerSynergy,
    RedFrostTemplarSynergy
}