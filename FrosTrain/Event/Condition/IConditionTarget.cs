using System;

public interface IConditionTarget
{
    #region PublicVariables
    public Action ConditionCheckAction { get; set; }
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public object GetConditionTarget(int index = 0);
	#endregion

	#region PrivateMethod
	#endregion
}
