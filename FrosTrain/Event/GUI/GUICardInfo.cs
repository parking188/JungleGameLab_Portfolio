using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Dynamic;

[RequireComponent(typeof(MouseAction))]
public class GUICardInfo : MonoBehaviour
{
    #region PublicVariables
    public Image bgImage;
    public Image bgImageSlot;
    public Image trainCarImage;
    public Image trainCarIconImage;
    public TMP_Text contextText;
    public TMP_Text levelText;
	public TMP_Text titleText;
    public Image rarityIconImage;
    public Image groupIconImage;
    public Image typeIconImage;
    public MouseAction mouseAction;
    public GameObject outline;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void InitGUICardInfo()
    {
        if (bgImage == null) bgImage = transform.Find("Card_BG").GetComponent<Image>();
        if (bgImageSlot == null) bgImageSlot = transform.Find("Card_BG/ImageSlot").GetComponent<Image>();
        if (trainCarImage == null) trainCarImage = transform.Find("Card_BG/ImageSlot/TrainCarImage").GetComponent<Image>();
        if (trainCarIconImage == null) trainCarIconImage = transform.Find("Card_BG/ImageSlot/TrainCarImage/TrainCar_Icon").GetComponent<Image>();
        if (contextText == null) contextText = transform.Find("Card_BG/ImageSlot/ContextBG/Effect_TMP").GetComponent<TMP_Text>();
        if (levelText == null) levelText = transform.Find("Card_BG/ImageSlot/LevelBG/Level_TMP").GetComponent<TMP_Text>();
        if (titleText == null) titleText = transform.Find("Card_BG/TitleBG/Title_TMP").GetComponent<TMP_Text>();
        if (rarityIconImage == null) rarityIconImage = transform.Find("Card_BG/RaritySlot/Rarity_Icon").GetComponent<Image>();
        if (groupIconImage == null) groupIconImage = transform.Find("Card_BG/IconSlot/IconSlot_Group/Group_Icon").GetComponent<Image>();
        if (typeIconImage == null) typeIconImage = transform.Find("Card_BG/IconSlot/IconSlot_Type/Type_Icon").GetComponent<Image>();
        if (mouseAction == null) mouseAction = GetComponent<MouseAction>();
        if (outline == null) outline = trainCarImage.transform.Find("Outline")?.gameObject;

        NullifyInfoData();
    }

    public void UpdateGUICardInfoData(SOTrainCar soTrainCar)
    {
        if (soTrainCar.carTags.Count > 1) bgImageSlot.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[1]].bgImageForCard;
        else bgImageSlot.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].bgImageForCard;
        trainCarImage.sprite = soTrainCar.cardSprite;
        if (soTrainCar.carTags.Count > 0) trainCarIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[0]].carIcon;
        else trainCarIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIcon;
        contextText.text = GameManager.GetLocalizingString(soTrainCar.context, soTrainCar.context_eng);
        if (soTrainCar.permanentHappiness > 0) contextText.text += GameManager.GetLocalizingString(" (현재 ", " (Current ") + soTrainCar.permanentHappiness.ToString() + ")";
        levelText.text = soTrainCar.level.ToString();
        titleText.text = GameManager.GetLocalizingString(soTrainCar.title, soTrainCar.title_eng);
        rarityIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.raritySprites[soTrainCar.tier - 1];
        if (soTrainCar.carTags.Count > 1) groupIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[1]].carIconForCard;
        else groupIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIconForCard;
        if (soTrainCar.carTags.Count > 0) typeIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[0]].carIconForCard;
        else typeIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIconForCard;
    }

    public void UpdateIfNullGUICardInfoData(SOTrainCar soTrainCar)
    {
        if (bgImageSlot.sprite == null && soTrainCar.carTags.Count > 1) bgImageSlot.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[1]].bgImageForCard;
        else if (bgImageSlot.sprite == null) bgImageSlot.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].bgImageForCard;
        if (trainCarImage.sprite == null) trainCarImage.sprite = soTrainCar.cardSprite;
        if (trainCarIconImage.sprite == null && soTrainCar.carTags.Count > 0) trainCarIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[0]].carIcon;
        else if (trainCarIconImage.sprite == null) trainCarIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIcon;
        contextText.text = GameManager.GetLocalizingString(soTrainCar.context, soTrainCar.context_eng);
        if (soTrainCar.permanentHappiness > 0) contextText.text += GameManager.GetLocalizingString(" (현재 ", " (Current ") + soTrainCar.permanentHappiness.ToString() + ")";
        if (levelText.text == null) levelText.text = soTrainCar.level.ToString();
        if (titleText.text == null) titleText.text = GameManager.GetLocalizingString(soTrainCar.title, soTrainCar.title_eng);
        if (rarityIconImage.sprite == null) rarityIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.raritySprites[soTrainCar.tier - 1];
        if (groupIconImage.sprite == null && soTrainCar.carTags.Count > 1) groupIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[1]].carIconForCard;
        else if (groupIconImage.sprite == null) groupIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIconForCard;
        if (typeIconImage.sprite == null && soTrainCar.carTags.Count > 0) typeIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[0]].carIconForCard;
        else if (typeIconImage.sprite == null) typeIconImage.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)ECarTag.Neutral].carIconForCard;
    }

    public void UpdateNonTrainCarInfoData(RewardsAndChance selectedReward)
    {
        bgImageSlot.sprite = null;
        trainCarImage.sprite = selectedReward.sprite;
        trainCarIconImage.sprite = null;
        contextText.text = GameManager.GetLocalizingString(selectedReward.context, selectedReward.context_eng);
        levelText.text = "";
        titleText.text = GameManager.GetLocalizingString(selectedReward.title, selectedReward.title_eng);
        rarityIconImage.sprite = null;
        groupIconImage.sprite = null;
        typeIconImage.sprite = null;
    }

    public void ShowSynergyInfoList(SOTrainCar soTrainCar)
    {
        for(int index = 0; index < soTrainCar.carTags.Count; index++)
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.ShowSynergyInfo(soTrainCar.carTags[index]);
        }
    }

    public void NullifyInfoData()
    {
        bgImageSlot.sprite = null;
        trainCarImage.sprite = null;
        trainCarIconImage.sprite = null;
        contextText.text = null;
        levelText.text = null;
        titleText.text = null;
        rarityIconImage.sprite = null;
        groupIconImage.sprite = null;
        typeIconImage.sprite = null;
    }

    public void UpdateSynergyInfoPosition(bool isWithScale = false, TextAnchor textAnchor = TextAnchor.UpperLeft)
    {
        Vector3 position = bgImage.transform.position;
        float offsetX = (((RectTransform)transform).rect.width / 2f) * MainCanvasController.Instance.transform.localScale.x;
        float offsetY = (((RectTransform)transform).rect.height / 2f) * MainCanvasController.Instance.transform.localScale.y;

        if (isWithScale)
        {
            offsetX *= transform.localScale.x * bgImage.transform.localScale.x;
            offsetY *= transform.localScale.y * bgImage.transform.localScale.y;
        }

        switch(textAnchor)
        {
            case TextAnchor.UpperLeft:
                ((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform)).pivot = Vector2.up;
                break;
            case TextAnchor.UpperRight:
                ((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform)).pivot = Vector2.up;
                offsetX *= -1;
                break;
            case TextAnchor.LowerLeft:
                ((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform)).pivot = Vector2.zero;
                offsetY *= 0;
                break;
            case TextAnchor.LowerRight:
                ((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform)).pivot = Vector2.zero;
                offsetX *= -1;
                offsetY *= 0;
                break;
        }

        position.x += offsetX;
        position.y += offsetY;

        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.AlignVerticalLayoutGroup(textAnchor);
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform.position = position;
    }

    public void HighlightCard(bool isOn)
    {
        if (outline == null) return;

        if (isOn)
        {
            outline.SetActive(true);
        }
        else
        {
            outline.SetActive(false);
        }
    }
    #endregion

    #region PrivateMethod
    #endregion
}
