using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class RewardResource : IReward
{
    #region PublicVariables
    public EResourceType resourceType;
    [Tooltip("ResourceType이 Population일 때 Add Mods 쓰면 오류 ")]
    public ERewardResourceBehaviour rewardBehaviour;
    [ShowIf("rewardBehaviour", ERewardResourceBehaviour.AddMods)]
    public float time;
    public float value;
    [ShowIf("rewardBehaviour", ERewardResourceBehaviour.AddMods)]
    public string titleEnum;
    public string titleEnum_eng;

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
        InitRewardTarget();
        InitRewardDelegate();
        isDispose = false;
        _value = value;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        switch (rewardBehaviour)
        {
            case ERewardResourceBehaviour.AddValue:
                giveReward = () => { ((ResourceBase)rewardTarget).AddValue(Mathf.FloorToInt(_value)); };
                break;
            case ERewardResourceBehaviour.AddMaxValue:
                giveReward = () => { ((ResourceBase)rewardTarget).AddMaxValue(Mathf.FloorToInt(_value)); };
                break;
            case ERewardResourceBehaviour.AddMods:
                giveReward = () => { ((ResourceModBase)rewardTarget).AddMods(new ModValueModifier(GameManager.GetLocalizingString(titleEnum, titleEnum_eng), this, _value, time)); };
                cancelReward = () => { ((ResourceModBase)rewardTarget).RemoveModsFrom(this); };
                break;
        }
    }

    private void InitRewardTarget()
    {
        rewardTarget = ResourceManager.Instance.resources[(int)resourceType];
    }

    public void Dispose()
    {
        isDispose = true;
        cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardResource()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }
    #endregion

    public enum ERewardResourceBehaviour
    {
        AddValue,
        AddMaxValue,
        AddMods
    }
}