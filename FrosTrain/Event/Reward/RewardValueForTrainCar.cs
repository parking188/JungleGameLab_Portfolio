using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RewardValueForTrainCar : IReward
{
    #region PublicVariables
    public EValueType valueType;
    [ShowIf("valueType", EValueType.ForTheFrontRoom)]
    public int count;
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
        switch(valueType)
        {
            case EValueType.MaxLaborer:
                giveReward = () => { 
                    ((TrainCar)rewardTarget).carContentBase.laborer.AddMaxValue(Mathf.FloorToInt(_value));
                    ((TrainCar)rewardTarget).soTrainCar.maxLaborer += Mathf.FloorToInt(_value);
                };
                cancelReward = () => { 
                    ((TrainCar)rewardTarget).carContentBase.laborer.AddMaxValue(-Mathf.FloorToInt(_value));
                    ((TrainCar)rewardTarget).soTrainCar.maxLaborer -= Mathf.FloorToInt(_value);
                };
                break;
            case EValueType.Range:
                giveReward = () => { ((TrainCar)rewardTarget).soTrainCar.range += Mathf.FloorToInt(_value); };
                cancelReward = () => { ((TrainCar)rewardTarget).soTrainCar.range -= Mathf.FloorToInt(_value); };
                break;
            case EValueType.MaxVisit:
                giveReward = () => { 
                    ((TrainCar)rewardTarget).trainCarVisitStack.conditionVisit.value += Mathf.FloorToInt(_value);
                    ((TrainCar)rewardTarget).trainCarVisitStack.UpdateVisitStackText();
                };
                cancelReward = () => {
                    ((TrainCar)rewardTarget).trainCarVisitStack.conditionVisit.value -= Mathf.FloorToInt(_value);
                    ((TrainCar)rewardTarget).trainCarVisitStack.UpdateVisitStackText();
                };
                break;
            case EValueType.PermanentHappiness:
                giveReward = () =>
                {
                    ((TrainCar)rewardTarget).soTrainCar.permanentHappiness += Mathf.FloorToInt(_value);
                };
                break;
            case EValueType.ForTheFrontRoom:
                giveReward = () =>
                {
                    TrainCar trainCar = (TrainCar)rewardTarget;
                    int backCount = trainCar.train.GetCarLastIndex() - trainCar.carIndex;
                    int permanentHappiness = (backCount / count) * Mathf.FloorToInt(_value);
                    trainCar.soTrainCar.permanentHappiness += permanentHappiness;
                };
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

    ~RewardValueForTrainCar()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion

    public enum EValueType
    {
        MaxLaborer,
        Range,
        MaxVisit,
        PermanentHappiness,
        ForTheFrontRoom
    }
}