using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class GUIRandomSelectButton : MonoBehaviour
{
    #region PublicVariables
    public GUICardInfo cardInfo;
    public Button button;
    public bool isSelected = false;
    public float selectedMovementTime = 0.5f;
    #endregion

    #region PrivateVariables
    private GUIRandomSelectPopup randomSelectPopup;
    private SOTrainCar soTrainCar;
	[SerializeField] private GameObject drone;
	[SerializeField] private GameObject card;
    #endregion

    #region PublicMethod
    public void InitSelectButton()
    {
        if (cardInfo == null) cardInfo = GetComponent<GUICardInfo>();
        if (randomSelectPopup == null) randomSelectPopup = GetComponentInParent<GUIRandomSelectPopup>();
        cardInfo.InitGUICardInfo();
        cardInfo.mouseAction.onMouseEnter += (eventData) => {
            if (soTrainCar == null) return;
            cardInfo.ShowSynergyInfoList(soTrainCar);

            if(transform.GetSiblingIndex() >= 2)
            {
                cardInfo.UpdateSynergyInfoPosition(false, TextAnchor.UpperRight);
            }
            else
            {
                cardInfo.UpdateSynergyInfoPosition(false, TextAnchor.UpperLeft);
            }
        };
        cardInfo.mouseAction.onMouseOver += () => { LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform)); };
        cardInfo.mouseAction.onMouseExit += (eventData) => {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();
        };
        //연출넣어주세요 카드선택애니메이션
        //Responsible for resetting and destroying a selected Button.
        //Panel Starting Drone Transition is located in MainCanvasController.cs
        //Remaining Card Animation is located in GUIRandomSelectPopup.cs
        button.onClick.AddListener(() => {
            isSelected = true;
            if (!SaveManager.Instance.isTuto) SaveManager.Instance.AddSteamUserStat(AchievementData.ACHIEVEMENT_STRING_02);
        });
    }
	public void TransitionSelectedButton()
	{
		button.DOKill();
		button.GetComponent<RectTransform>().DOAnchorPosY(Screen.height, selectedMovementTime).SetUpdate(true).SetEase(Ease.OutCubic) //=>Selected Card Movement
		.OnComplete(() => {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();
			Destroy(gameObject);
		});
	}

    public void InitSelectButtonData(SOTrainCar _soTrainCar)
    {
        soTrainCar = _soTrainCar;
        cardInfo.UpdateGUICardInfoData(soTrainCar);
        UpdateHighlight();
        TrashCan.Instance.AfterDeleteAction += UpdateHighlight;
    }

	public void DestoryUnselectedButton(float _transitionDuration)
	{
		Sequence sequence = DOTween.Sequence();

		drone.transform.SetParent(transform.parent.parent);
		sequence.Append(((RectTransform)drone.transform).DOAnchorPosY(1200, _transitionDuration).SetEase(Ease.InBack))
			.Join(card.transform.DORotate(new Vector3(0, 0, Random.Range(-30, 30)), _transitionDuration))
			.Join(((RectTransform)card.transform).DOAnchorPosY(-1200, _transitionDuration).SetEase(Ease.InQuint))
			.SetUpdate(true)
			.OnComplete(() =>
			{
				Destroy(drone);
				Destroy(gameObject);
			});
	}
	public void SetInteractablity(bool b)
	{
		cardInfo.bgImage.raycastTarget = b;
	}
    #endregion

    #region PrivateMethod
    private void UpdateHighlight()
    {
        if (soTrainCar == null) return;

        List<TrainCar> trainCarList = GameManager.Instance.player.train.GetCarList();
        for (int index = 0; index < trainCarList.Count; index++)
        {
            if (trainCarList[index].soTrainCar.title.Equals(soTrainCar.title))
            {
                cardInfo.HighlightCard(true);
                return;
            }
        }

        List<InventoryCard> cardList = GameManager.Instance.player.inventory.inventoryCards;
        for (int index = 0; index < cardList.Count; index++)
        {
            if (cardList[index].soTrainCar.title.Equals(soTrainCar.title))
            {
                cardInfo.HighlightCard(true);
                return;
            }
        }

        cardInfo.HighlightCard(false);
    }

    private void OnDestroy()
    {

        if(TrashCan.Instance != null)
            TrashCan.Instance.AfterDeleteAction -= UpdateHighlight;
    }
    #endregion
}
