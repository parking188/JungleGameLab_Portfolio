using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Spine;

public class GUITrainCarHoverSynergyInfo : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    [SerializeField] private List<GameObject> synergyInfoList = new List<GameObject>();
    private List<TMP_Text> synergyInfoTextList = new List<TMP_Text>();
    private GUISynergyPanel guiSynergyPanel;
    private VerticalLayoutGroup verticalLayoutGroup;
    #endregion

    #region PublicMethod
    public void InitGUITrainCarHoverSynergyInfo()
    {
        guiSynergyPanel = MainCanvasController.Instance.guiSynergyPanel;
        for (int index = 0; index < synergyInfoList.Count; index++)
        {
            synergyInfoTextList.Add(synergyInfoList[index].transform.GetComponentInChildren<TMP_Text>());
        }
        HideAllSynergyInfo();
        GameManager.Instance.player.train.AddOrDeleteTrainCarAction += OffAllHighlight;
        GameManager.Instance.player.train.AddOrDeleteTrainCarAction += HighlightUpgradableAuto;
        if (verticalLayoutGroup == null) verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    public void ShowSynergyInfo(ECarTag carTag)
    {
        if (carTag == ECarTag.Neutral || carTag == ECarTag.Max) return;
        synergyInfoList[(int)carTag].SetActive(true);
    }

    public void HideAllSynergyInfo()
    {
        for (int index = 0; index < synergyInfoList.Count; index++)
        {
            synergyInfoList[index].SetActive(false);
        }
    }

    public void UpdateSynergyInfo(ECarTag carTag)
    {
        if (carTag == ECarTag.Neutral || carTag == ECarTag.Max) return;
        SOTrainSynergyText soTrainSynergyText = guiSynergyPanel.soTrainSynergyTexts[(int)carTag];
        GUISynergy guiSynergy = guiSynergyPanel.guiSynergies[(int)carTag];

        string text = "<size=23><b><color=#FFFFFF>" + "<style=" + soTrainSynergyText.carTag.ToString() + "> " + guiSynergy.synergyNameText.text + "</color></b></size><br>";
        for (int index = 0; index < soTrainSynergyText.levelContexts.Count; index++)
        {
            if(soTrainSynergyText.levelContexts[index].Equals(""))
            {
                continue;
            }

            string levelContext = GameManager.GetLocalizingString(soTrainSynergyText.levelContexts[index], soTrainSynergyText.levelContexts_eng[index]);
            if(index == guiSynergy.level || index == 0)
            {
                levelContext = "<b><color=#FFFFFF>" + levelContext + "</color></b>";
            }
            text += levelContext + "<br>";
        }

        synergyInfoTextList[(int)carTag].text = text;
    }

    public void AlignVerticalLayoutGroup(TextAnchor textAnchor)
    {
        verticalLayoutGroup.childAlignment = textAnchor;
    }

    public void HighlightSynergy(ECarTag carTag, bool isOn = true)
    {
        GameManager.Instance.player.train.HighlightSynergyTrainCar(carTag, isOn);
        GameManager.Instance.player.inventory.HighlightSynergyCard(carTag, isOn);
    }

    public void HighlightSame(SOTrainCar soTrainCar, bool isOn = true)
    {
        GameManager.Instance.player.train.HighlightSameTrainCar(soTrainCar, isOn);
        GameManager.Instance.player.inventory.HighlightSameCard(soTrainCar, isOn);
    }

    public void HighlightUpgradableAuto()
    {
        GameManager.Instance.player.train.HighlightUpgradableTrainCarAuto();
        GameManager.Instance.player.inventory.HighlightUpgradableCardAuto();
    }

    public void OffAllHighlight()
    {
        GameManager.Instance.player.train.OffAllHighlightTrainCar();
        GameManager.Instance.player.inventory.OffAllHighlightCard();
    }
    #endregion

    #region PrivateMethod
    #endregion
}
