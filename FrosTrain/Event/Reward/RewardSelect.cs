using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RewardSelect : IReward
{
    #region PublicVariables
    public bool isTrainLevelBased = false;
    public bool isOptionCountByTrain = false;

    [HideIf("@isOptionCountByTrain == true")]
    public int optionCount = 3;
    [HideIf("@isTrainLevelBased == true")]
    public List<int> tiers = new List<int>();
    [HideIf("@isTrainLevelBased == true")]
    public List<ECarTag> tags = new List<ECarTag>();
    [HideIf("@isTrainLevelBased == true")]
    public List<SORandomRewards.RewardOptionTypes> optionTypes;

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
        InitRewardDelegate();
        isDispose = false;
    }

    public void SelectTierRandom()
    {
        if(rewardTarget is Train)
        {
            Train train = (Train)rewardTarget;
            tiers.Clear();

            float maxChance = 0f;
            List<float> tierChances = train.tierChanceByLevel[train.level].tierChances;
            for (int tier = 0; tier < tierChances.Count; tier++)
            {
                maxChance += tierChances[tier];
            }

            float randomChance = UnityEngine.Random.Range(0f, maxChance);
            float curChance = 0f;
            for (int tier = 0; tier < tierChances.Count; tier++)
            {
                if (randomChance >= curChance && randomChance < curChance + tierChances[tier])
                {
                    tiers.Add(tier + 1);
                    break;
                }

                curChance += tierChances[tier];
            }
        }
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        giveReward = () => {
            if (isOptionCountByTrain)
                optionCount = GameManager.Instance.player.train.selectCardOptionCount;

            GameEventManager.Instance.PopupRewardSelect(this);
        };
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardSelect()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        if (isTrainLevelBased)
        {
            rewardTarget = GameManager.Instance.player.train;

            tiers.Clear();
            tags.Clear();
            optionTypes.Clear();

            optionTypes.Add(SORandomRewards.RewardOptionTypes.TrainCar);
        }
        else
        {
            rewardTarget = _rewardTarget;
        }
    }
    #endregion
}