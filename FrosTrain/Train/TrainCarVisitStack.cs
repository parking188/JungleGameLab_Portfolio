using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrainCarVisitStack : MonoBehaviour
{
	#region PublicVariables
	public SpriteRenderer sprite;
	public TMP_Text visitStackText;
    [HideInInspector] public ConditionTrainCar conditionVisit;
    #endregion

    #region PrivateVariables
    private int showCurrentStack = 0;
    #endregion

    #region PublicMethod
    public void InitTrainCarVisitStack(ConditionTrainCar _conditionVisit)
    {
        conditionVisit = _conditionVisit;
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (visitStackText == null ) visitStackText = GetComponentInChildren<TMP_Text>();

        SetVisitStackText(0, _conditionVisit.value);
    }

    public void SetVisitStackText(int currentStack, int maxStack)
    {
        visitStackText.text = currentStack.ToString() + "/" + maxStack.ToString();
    }

    public void ShowIncreaseStack(int currentLaborer)
    {
        showCurrentStack += conditionVisit.gainedVisit / currentLaborer;
        if (showCurrentStack >= conditionVisit.value) showCurrentStack -= conditionVisit.value;
        SetVisitStackText(showCurrentStack, conditionVisit.value);
    }

    public void UpdateVisitStackText()
    {
        SetVisitStackText(conditionVisit.visit, conditionVisit.value);
        showCurrentStack = conditionVisit.visit;
        conditionVisit.gainedVisit = 0;
    }

    public void ResetGainedVisit()
    {
        conditionVisit.gainedVisit = 0;
    }
    #endregion

    #region PrivateMethod
    #endregion
}
