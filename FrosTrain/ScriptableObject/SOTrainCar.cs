using Unity.VisualScripting;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TrainCar Data", menuName = "ScriptableObjects/Train/TrainCar")]
public class SOTrainCar : ScriptableObject
{
    #region PublicVariables
    [Header("Initial Data")]
    public Vector2 carSize;
	public float carInterval;
	public Sprite sprite;
    public Sprite cardSprite;
    public SkeletonDataAsset skeletonDataAsset;
    public bool isSecurityTrainCar = false;
	public bool isIvoryTower = false;

    [Header("Value Data")]
    public SOTrainCar nextSOTrainCar;
    public int range = 0;
	public int maxLaborer = 0;
    public int tier;
    public int level;
    [HideInInspector] public int permanentHappiness = 0;
    [HideInInspector] public float training = 0f;
    public List<ECarTag> carTags;
    [SerializeReference] public List<IReward> carSynergys = new List<IReward>();
    [SerializeReference] public List<IReward> rewards = new List<IReward>();
    [SerializeReference] public List<IReward> additionalRewards = new List<IReward>();

    [Header("Text")]
    public string title;
    [TextArea(3, 10)] public string context;
    [TextArea(3, 10)] public string flavorText;
    public List<SOText> synergyTexts = new List<SOText>();

    [Header("Text_Eng")]
    public string title_eng;
    [TextArea(3, 10)] public string context_eng;
    [TextArea(3, 10)] public string flavorText_eng;
    public List<SOText> synergyTexts_eng = new List<SOText>();
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public SOTrainCar Clone()
    {
        SOTrainCar soTrainCar = Instantiate(this);
        soTrainCar.name = soTrainCar.name.TrimEnd("(Clone)");
        return soTrainCar;
    }

    public string GetTagText()
    {
        string tagText = "";
        
        for(int index = 0; index < carTags.Count; index++)
        {
            switch((ELanguage)PlayerPrefs.GetInt(SaveManager.SAVEKEY_LANGUAGE, 1))
            {
                case ELanguage.Korean:
                    switch (carTags[index])
                    {
                        case ECarTag.Education:
                            tagText += "����";
                            break;
                        case ECarTag.Security:
                            tagText += "ġ��";
                            break;
                        case ECarTag.Culture:
                            tagText += "��ȭ";
                            break;
                        case ECarTag.Entertainment:
                            tagText += "����";
                            break;
                        case ECarTag.TrainPower:
                            tagText += "����";
                            break;
                        case ECarTag.Media:
                            tagText += "���";
                            break;
                        case ECarTag.Large:
                            tagText += "����";
                            break;
                        case ECarTag.Communication:
                            tagText += "���";
                            break;
                        case ECarTag.CentralSupplyBureau:
                            tagText += "�Ѻ��ް�����";
                            break;
                        case ECarTag.FirstClass:
                            tagText += "�۽�ƮŬ����";
                            break;
                        case ECarTag.FuelAndHammerBrotherhood:
                            tagText += "����� ��ġ ��а��";
                            break;
                        case ECarTag.RedFrostTemplar:
                            tagText += "�������� ����";
                            break;
                        case ECarTag.ChildrenOfEngine:
                            tagText += "������ �ڳ��";
                            break;
                        case ECarTag.Academy:
                            tagText += "��ī����";
                            break;
                        case ECarTag.GuildOfExplorers:
                            tagText += "Ž�谡����";
                            break;
                    }
                    break;
                case ELanguage.English:
                    tagText += carTags[index].ToString();
                    break;
            }

            tagText += '/';
        }
        tagText = tagText.TrimEnd('/');
        return tagText;
    }

    public bool CompareCarTags(List<ECarTag> _carTags)
    {
        if (_carTags.Count <= 0) return true;

        for(int originIndex = 0; originIndex < carTags.Count; originIndex++)
        {
            for(int listIndex = 0; listIndex < _carTags.Count; listIndex++)
            {
                if (_carTags[listIndex] == carTags[originIndex])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CompareCarTags(ECarTag _carTag)
    {
        for (int originIndex = 0; originIndex < carTags.Count; originIndex++)
        {
            if(_carTag == carTags[originIndex])
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region PrivateMethod
    #endregion
}

public enum ECarTag
{
    Education,
    Security,
    Culture,
    Entertainment,
    TrainPower,
    Media,
    Large,
    Communication,
    CentralSupplyBureau,
    FirstClass,
    FuelAndHammerBrotherhood,
    RedFrostTemplar,
    ChildrenOfEngine,
    Academy,
    GuildOfExplorers,
	Neutral,
    Max
}