using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TrainTrait Data", menuName = "ScriptableObjects/Train/TrainTrait")]
public class SOTrainTrait : ScriptableObject
{
    #region PublicVariables
    [Header("Context Data")]
    public int tier = 1;
	public Sprite sprite;
    public string title;
    [TextArea(3, 10)] public string context;
    [TextArea(3, 10)] public string flavorText;

    [Header("Trait Data")]
    [SerializeReference] public List<IReward> rewards = new List<IReward>();

    [Header("Text_Eng")]
    public string title_eng;
    [TextArea(3, 10)] public string context_eng;
    [TextArea(3, 10)] public string flavorText_eng;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SOTrainTrait Clone()
    {
        return Instantiate(this);
    }
    #endregion

    #region PrivateMethod
    #endregion
}