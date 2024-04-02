using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TrainSynergyText Data", menuName = "ScriptableObjects/Train/TrainSynergyText")]
public class SOTrainSynergyText : ScriptableObject
{
    #region PublicVariables
    [Header("Synergy Data")]
    public ECarTag carTag;
    public Sprite carIcon;
    public Sprite carIconForCard;
    public Sprite bgImageForCard;
    public List<Sprite> spriteList;
    public List<int> levelScales = new List<int>();
    public List<ELevelColor> levelColorList = new List<ELevelColor>();

    [Header("Text_Kor")]
    public string title;
    [SerializeField] public List<string> levelContexts = new List<string>();
    [TextArea(3, 10)] public string flavorText;

    [Header("Text_Eng")]
    public string title_eng;
    [SerializeField] public List<string> levelContexts_eng = new List<string>();
    [TextArea(3, 10)] public string flavorText_eng;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SOTrainSynergyText Clone()
    {
        return Instantiate(this);
    }
    #endregion

    #region PrivateMethod
    #endregion
}

public enum ELevelColor
{
	None,
    Bronze,
    Silver,
    Gold,
    Platinum
}