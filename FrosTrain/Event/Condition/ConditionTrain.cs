using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class ConditionTrain : ICondition
{
    #region PublicVariables
    public ETrainCondition condition;

    [ShowIf("condition", ETrainCondition.CarTagCount)]
    public ECarTag carTag;
    [ShowIf("condition", ETrainCondition.CarTagCount)]
    public EComparisonOperator @operator;
    [ShowIf("condition", ETrainCondition.CarTagCount)]
    public int tagCount;

    public IConditionTarget conditionTarget { get; set; }
    public bool isConditionMet { get; set; }
    public Action actionWhenConditionMet { get; set; }
    #endregion

    #region PrivateVariables
    private delegate bool CompareDelegate();
    private CompareDelegate compareValue;
    private bool isDispose;
    private Train train;
    #endregion

    #region PublicMethod
    public void InitCondition(IConditionTarget _conditionTarget = null)
    {
        conditionTarget = _conditionTarget;
        InitCompareDelegate();
        isConditionMet = false;
        isDispose = false;
        train = GameManager.Instance.player.train;
    }

    ~ConditionTrain()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        isDispose = true;
        GC.SuppressFinalize(this);
    }

    public void CheckCondition()
    {
        isConditionMet = compareValue();
        if(isConditionMet)
        {
            actionWhenConditionMet?.Invoke();
        }
    }
    #endregion

    #region PrivateMethod
    private void InitCompareDelegate()
    {
        switch (condition)
        {
            case ETrainCondition.CarTagCount:
                switch(@operator)
                {
                    case EComparisonOperator.Equal:
                        compareValue = () => { return train.carTagCountList[(int)carTag] == tagCount; };
                        break;
                    case EComparisonOperator.NotEqual:
                        compareValue = () => { return train.carTagCountList[(int)carTag] != tagCount; };
                        break;
                    case EComparisonOperator.GreaterThan:
                        compareValue = () => { return train.carTagCountList[(int)carTag] > tagCount; };
                        break;
                    case EComparisonOperator.LessThan:
                        compareValue = () => { return train.carTagCountList[(int)carTag] < tagCount; };
                        break;
                    case EComparisonOperator.GreaterOrEqual:
                        compareValue = () => { return train.carTagCountList[(int)carTag] >= tagCount; };
                        break;
                    case EComparisonOperator.LessOrEqual:
                        compareValue = () => { return train.carTagCountList[(int)carTag] <= tagCount; };
                        break;
                }
                break;
        }
    }
    #endregion

    public enum ETrainCondition
    {
        CarTagCount
    }
}