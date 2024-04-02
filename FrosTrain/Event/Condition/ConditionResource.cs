using System;
using UnityEngine;

public class ConditionResource : ICondition
{
    #region PublicVariables
    public EResourceType resourceType;
    public ResourceModBase.ETargetDataIndex targetDataIndex;
    public EComparisonOperator @operator;
    public float value;

    public IConditionTarget conditionTarget { get; set; }
    public bool isConditionMet { get; set; }
    public Action actionWhenConditionMet { get; set; }
    #endregion

    #region PrivateVariables
    private object targetValue;
    private delegate bool CompareDelegate();
    private CompareDelegate compareValue;
    private bool isDispose;
    #endregion

    #region PublicMethod
    public void InitCondition(IConditionTarget _conditionTarget = null)
    {
        InitConditionTarget();
        InitCompareDelegate();
        conditionTarget.ConditionCheckAction += CheckCondition;
        isConditionMet = false;
        isDispose = false;
        isConditionMet = compareValue();
    }

    ~ConditionResource()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        isDispose = true;
        if (conditionTarget.ConditionCheckAction != null)
        {
            conditionTarget.ConditionCheckAction -= CheckCondition;
        }
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
        switch(@operator)
        {
            case EComparisonOperator.Equal:
                compareValue = () => {
                    targetValue = conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) == 0) ? true : false; 
                };
                break;
            case EComparisonOperator.NotEqual:
                compareValue = () => {
                    targetValue = conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) != 0) ? true : false; 
                };
                break;
            case EComparisonOperator.GreaterThan:
                compareValue = () => {
                    targetValue = conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) < 0) ? true : false; 
                };
                break;
            case EComparisonOperator.LessThan:
                compareValue = () => {
                    targetValue = conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) > 0) ? true : false; 
                };
                break;
            case EComparisonOperator.GreaterOrEqual:
                compareValue = () => {
                    targetValue = conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) <= 0) ? true : false; 
                };
                break;
            case EComparisonOperator.LessOrEqual:
                compareValue = () => {
                    targetValue = (IComparable)conditionTarget.GetConditionTarget((int)targetDataIndex);
                    return (value.CompareTo((int)targetValue) >= 0) ? true : false;
                };
                break;
        }
    }

    private void InitConditionTarget()
    {
        conditionTarget = ResourceManager.Instance.resources[(int)resourceType];
    }
    #endregion
}
public enum EComparisonOperator
{
    [InspectorName("==")]
    Equal,
    [InspectorName("!=")]
    NotEqual,
    [InspectorName(">")]
    GreaterThan,
    [InspectorName("<")]
    LessThan,
    [InspectorName(">=")]
    GreaterOrEqual,
    [InspectorName("<=")]
    LessOrEqual
}