using System;
using UnityEngine;

public interface ICondition : IDisposable
{
    #region PublicVariables
    public IConditionTarget conditionTarget { get; set; }
    [HideInInspector] public bool isConditionMet { get; set; }
    public Action actionWhenConditionMet { get; set; }
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public abstract void InitCondition(IConditionTarget _conditionTarget = null);
    public abstract void CheckCondition();
    #endregion

    #region PrivateMethod
    #endregion
}