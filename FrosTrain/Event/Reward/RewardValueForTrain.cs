using Sirenix.OdinInspector;
using System;
using UnityEngine;
using static RewardValueForTrainCar;

public class RewardValueForTrain : IReward
{
    #region PublicVariables
    public EActivateTiming activateTiming;
    public ESynergyType synergyType;
    public ETrainReward trainRewardType;
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
        _value = value;
        InitRewardTarget(_rewardTarget);
        InitRewardDelegate();
        RegistActivateTiming();
        isDispose = false;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        switch (trainRewardType)
        {
            case ETrainReward.Exp:
                giveReward = () => { GameManager.Instance.player.train.AddExp(_value); };
                break;
            case ETrainReward.EngineExp:
                giveReward = () => { ((TrainCar)rewardTarget).carContentBase.AddEngineExpWithCycle(_value * ((TrainCar)rewardTarget).carContentBase.trainPowerSynergyMultiple); };
                break;
            case ETrainReward.LaborCycleTime:
                GameManager.Instance.player.train.laborCycleTimeWeightTrainCar += _value;
                cancelReward = () => { GameManager.Instance.player.train.laborCycleTimeWeightTrainCar -= _value; };
                break;
            case ETrainReward.TrainingMultipleByTrainCar:
                GameManager.Instance.player.train.trainingMultipleByTrainCar += _value;
                cancelReward = () => { GameManager.Instance.player.train.trainingMultipleByTrainCar -= _value; };
                break;
            case ETrainReward.TrainSpeed:
                GameManager.Instance.player.train.AddTrainSpeed(_value);
                break;
        }

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

    ~RewardValueForTrain()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion

    public enum ETrainReward
    {
        Exp,
        EngineExp,
        LaborCycleTime,
        TrainingMultipleByTrainCar,
        TrainSpeed
    }
}