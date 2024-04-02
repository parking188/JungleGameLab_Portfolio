using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TrainSynergy Data", menuName = "ScriptableObjects/Train/TrainSynergy")]
public class SOTrainSynergy : ScriptableObject
{
    #region PublicVariables
    [Header("Context Data")]
    public ECarTag CarTag;
    public string title;

    [Header("Synergy Data")]
    [SerializeReference] public List<IReward> rewards = new List<IReward>();
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SOTrainSynergy Clone()
    {
        return Instantiate(this);
    }
    #endregion

    #region PrivateMethod
    #endregion
}