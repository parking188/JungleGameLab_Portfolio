using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Event/RandomRewards", fileName = "New RandomRewards Data")]
public class SORandomRewards : ScriptableObject
{
    #region PublicVariables
    public int tier;
    public RewardOptionTypes optionType;
    public List<RewardsAndChance> rewards = new List<RewardsAndChance>();
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SORandomRewards Clone()
    {
        return Instantiate(this);
    }

    public bool CheckOptionTypes(List<RewardOptionTypes> optionTypes)
    {
        for(int index = 0; index < optionTypes.Count; index++)
        {
            if (optionTypes[index] == optionType)
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckTiers(List<int> checkTiers)
    {
        for(int index = 0; index < checkTiers.Count; index++)
        {
            if (checkTiers[index] == tier)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region PrivateMethod
    #endregion

    public enum RewardOptionTypes
    {
        Resource,
        TrainCar,
        TrainTrait
    }
}

[Serializable]
public class RewardsAndChance
{
    [HideIf("@(this.rewards.Count > 0 && this.rewards[0] is RewardTrainCar)")]
    public Sprite sprite;
    [HideIf("@(this.rewards.Count > 0 && this.rewards[0] is RewardTrainCar)")]
    public string title;
    [HideIf("@(this.rewards.Count > 0 && this.rewards[0] is RewardTrainCar)")]
    [TextArea] public string context;

    [SerializeReference] public List<IReward> rewards;
    public float chance;

    [Header("Text_Eng")]
    [HideIf("@(this.rewards.Count > 0 && this.rewards[0] is RewardTrainCar)")]
    public string title_eng;
    [HideIf("@(this.rewards.Count > 0 && this.rewards[0] is RewardTrainCar)")]
    [TextArea] public string context_eng;
}