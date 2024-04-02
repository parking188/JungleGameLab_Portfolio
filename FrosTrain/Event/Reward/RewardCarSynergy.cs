using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class RewardCarSynergy : IReward
{
    #region PublicVariables
    public ESynergyType synergyType;
    [ShowIf("synergyType", ESynergyType.Link)]
    public bool isActiveWhenTargetCarOff = false;
    [ShowIf("synergyType", ESynergyType.Link)]
    public ECarTag targetCarTag;
    [ShowIf("@(this.synergyType == ESynergyType.Zeal) || (this.synergyType == ESynergyType.MinimumLaborer) || (this.synergyType == ESynergyType.MultipleLabor)")]
    public float value;
    [ShowIf("@(this.synergyType == ESynergyType.Link) || (this.synergyType == ESynergyType.Zeal)")]
    [SerializeReference] public List<IReward> rewards;

    public EActivateTiming _activateTiming { get; set; }
    public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    private bool isDispose;
    private List<TrainCar> targetTrainCarList;
    #endregion

    #region PublicMethod
    public void InitReward(IRewardTarget _rewardTarget = null)
    {
        InitRewardTarget(_rewardTarget);
        InitRewardList(_rewardTarget);
        InitRewardDelegate();
        isDispose = false;
        _value = value;
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardCarSynergy()
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
        switch (synergyType)
        {
            case ESynergyType.Link:
                giveReward = () => {
                    TrainCar trainCar = ((TrainCar)rewardTarget);
                    int range = trainCar.soTrainCar.range;
                    targetTrainCarList = SearchCarTagInRange(range, targetCarTag);
                    for(int index = 0; index < targetTrainCarList.Count; index++)
                    {
                        InitRewardList(targetTrainCarList[index]);
                        GiveRewards();
                    }
                    return; 
                };
                cancelReward = () => {
                    for(int index = 0; index < targetTrainCarList.Count; index++)
                    {
                        InitRewardList(targetTrainCarList[index]);
                        CancelRewards();
                    }
                    return; 
                };
                break;
            case ESynergyType.Zeal:
                giveReward = () => {
                    InitRewardListValueToZeal(ResourceManager.Instance.GetZeal().Value);
                    GiveRewards();
                };
                break;
            case ESynergyType.MinimumLaborer:
                giveReward = () =>
                {
                    CarContentBase carContentBase = ((TrainCar)rewardTarget).carContentBase;
                    if(carContentBase.laborer.Value < _value)
                    {
                        int addLaborerCount = Mathf.FloorToInt(_value) - carContentBase.laborer.Value;
                        carContentBase.laborer.AddValue(addLaborerCount);
                        carContentBase.isLaborerFull = (carContentBase.laborer.Value >= carContentBase.laborer.MaxValue) ? true : false;
                        ((TrainCar)rewardTarget).isGetSynergy = true;
                    }
                };
                break;
            case ESynergyType.MultipleLabor:
                giveReward = () =>
                {
                    CarContentBase carContentBase = ((TrainCar)rewardTarget).carContentBase;
                    carContentBase.laborCount += Mathf.FloorToInt(_value);
                };
                cancelReward = () =>
                {
                    CarContentBase carContentBase = ((TrainCar)rewardTarget).carContentBase;
                    carContentBase.laborCount -= Mathf.FloorToInt(_value);
                };
                break;
        }
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        rewardTarget = _rewardTarget;
    }

    private void InitRewardList(IRewardTarget _rewardTarget = null)
    {
        for(int index = 0; index < rewards.Count; index++)
        {
            if (rewards[index] is RewardHappinessForTrainCar)
            {
                rewards[index].InitReward(rewardTarget);
            }
            else
            {
                rewards[index].InitReward(_rewardTarget);
            }
        }
    }

    private void InitRewardListValueToZeal(int zeal)
    {
        for(int index = 0; index < rewards.Count; index++)
        {
            rewards[index]._value = zeal * _value;
        }
    }

    private List<TrainCar> SearchCarTagInRange(int range, ECarTag carTag)
    {
        List<TrainCar> targetTrainCars = new List<TrainCar>();
        TrainCar trainCar = ((TrainCar)rewardTarget);

        for (int index = 1; index <= range; index++)
        {
            AddTargetTrainCar(trainCar.carIndex + index, carTag, targetTrainCars);
            AddTargetTrainCar(trainCar.carIndex - index, carTag, targetTrainCars);
        }

        return targetTrainCars;
    }

    private void AddTargetTrainCar(int index, ECarTag carTag, List<TrainCar> targetTrainCars)
    {
        if (CheckCarIndex(index))
        {
            TrainCar targetTrainCar = GameManager.Instance.player.train.GetCarList()[index];
            if (targetTrainCar.CheckCarTags(carTag) &&
                (targetTrainCar.carContentBase.laborer.Value > 0 || isActiveWhenTargetCarOff))
            {
                targetTrainCars.Add(targetTrainCar);
            }
        }
    }

    private bool CheckCarIndex(int index)
    {
        return (index > 0) && (index <= GameManager.Instance.player.train.GetCarLastIndex());
    }

    private void GiveRewards()
    {
        for(int index = 0; index < rewards.Count; index++)
        {
            rewards[index].giveReward?.Invoke();
        }
    }

    private void CancelRewards()
    {
        for(int index = 0; index < rewards.Count; index++)
        {
            rewards[index].cancelReward?.Invoke();
        }
    }
    #endregion

    public enum ESynergyType
    {
        Link,
        Zeal,
        MinimumLaborer,
        MultipleLabor
    }
}