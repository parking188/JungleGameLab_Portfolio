using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class ConditionTrainCar : ICondition
{
    #region PublicVariables
    public ETrainCarCondition condition;

    [ShowIf("condition", ETrainCarCondition.CarType)]
    public SOTrainCar carType;
    [ShowIf("@(this.condition == ETrainCarCondition.Range) || (this.condition == ETrainCarCondition.CurLaborer) || (this.condition == ETrainCarCondition.MaxLaborer) || (this.condition == ETrainCarCondition.Tier)")]
    public EComparisonOperator @operator;
    [ShowIf("@(this.condition == ETrainCarCondition.Range) || (this.condition == ETrainCarCondition.CurLaborer) || (this.condition == ETrainCarCondition.MaxLaborer) || (this.condition == ETrainCarCondition.Tier) || (this.condition == ETrainCarCondition.Visit)")]
    public int value;

    public IConditionTarget conditionTarget { get; set; }
    public bool isConditionMet { get; set; }
    public Action actionWhenConditionMet { get; set; }
    #endregion

    #region PrivateVariables
    private delegate bool CompareDelegate();
    private CompareDelegate compareValue;
    private bool isDispose;
    [ShowIf("condition", ETrainCarCondition.Visit)]
    public int visit { get; private set; }
    [HideInInspector] public int gainedVisit;
    private bool isVisitStackOn = false;
    #endregion

    #region PublicMethod
    public void InitCondition(IConditionTarget _conditionTarget = null)
    {
        conditionTarget = _conditionTarget;
        InitCompareDelegate();
        isConditionMet = false;
        isDispose = false;

        if(condition == ETrainCarCondition.Visit && isVisitStackOn == false)
        {
            ((TrainCar)conditionTarget).InitTrainCarVisitStack(this);
            gainedVisit = 0;
            isVisitStackOn = true;
        }
    }

    ~ConditionTrainCar()
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
            case ETrainCarCondition.Visit:
                compareValue = () => {
                    visit++;
                    gainedVisit++;
                    if (visit >= value)
                    {
                        visit -= value;
                        return true;
                    }
                    return false; 
                };
                break;
            case ETrainCarCondition.Packed:
                compareValue = () => { return ((TrainCar)conditionTarget).carContentBase.isLaborerFull; };
                break;
            case ETrainCarCondition.Meditation:
                compareValue = () => { return ((TrainCar)conditionTarget).carContentBase.laborer.Value == 1; };
                break;
            case ETrainCarCondition.CarType:
                compareValue = () => { return ((TrainCar)conditionTarget).soTrainCar.name.Contains(carType.name); };
                break;
            case ETrainCarCondition.Range:
            case ETrainCarCondition.CurLaborer:
            case ETrainCarCondition.MaxLaborer:
            case ETrainCarCondition.Tier:
                switch(@operator)
                {
                    case EComparisonOperator.Equal:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) == value; };
                        break;
                    case EComparisonOperator.NotEqual:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) != value; };
                        break;
                    case EComparisonOperator.GreaterThan:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) > value; };
                        break;
                    case EComparisonOperator.LessThan:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) < value; };
                        break;
                    case EComparisonOperator.GreaterOrEqual:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) >= value; };
                        break;
                    case EComparisonOperator.LessOrEqual:
                        compareValue = () => { return (int)(conditionTarget.GetConditionTarget((int)condition)) <= value; };
                        break;
                }
                break;
        }
    }
    #endregion

    public enum ETrainCarCondition
    {
        Visit,
        Packed,
        Meditation,
        CarType,
        Range,
        CurLaborer,
        MaxLaborer,
        Tier
    }
}