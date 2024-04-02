using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCard : MonoBehaviour
{
    #region PublicVariables
    public GUICardInfo cardInfo;
    public Image raycastImage;
    public int transformOrder;
    #endregion

    #region PrivateVariables
    [HideInInspector] public SOTrainCar soTrainCar { get; private set; }
    private Transform inventoryTransform;
    private Train train;
    private PlayerController playerController;

	[Header("Hovering")]
	[SerializeField] private GameObject hoveringCard;
	[SerializeField] private Vector2 hoveringScale;
	[SerializeField] private float hoveringPosY;
	[SerializeField] private Canvas sortingCanvas;
	[SerializeField] private Canvas outlineSortingCanvas;
	private const int SORTING_ORDER_IDLE = 5;
	private const int SORTING_ORDER_HOVER = 100;
    #endregion

    #region PublicMethod
    public void InitInventoryCard()
    {
        if (cardInfo == null) cardInfo = GetComponent<GUICardInfo>();
        if (raycastImage == null) raycastImage = transform.Find("Raycast").GetComponent<Image>();
        inventoryTransform = transform.parent;
        train = GameManager.Instance.player.train;
        cardInfo.InitGUICardInfo();
        playerController = PlayerController.Instance;
		sortingCanvas.sortingLayerName = "UI";
		outlineSortingCanvas.sortingLayerName = "UI";
        InitMouseAction();
    }

    public void InitCardData(SOTrainCar _soTrainCar)
    {
        soTrainCar = _soTrainCar; 
        cardInfo.UpdateGUICardInfoData(soTrainCar);
        DataManager.Instance.SaveInventory(GameManager.Instance.player.inventory.inventoryCards);
    }

    public void ExitInventory()
    {
        playerController.dragObject.InitDatas(soTrainCar);
        playerController.dragObject.TurnCard();
        cardInfo.bgImage.gameObject.SetActive(false);
        transformOrder = transform.GetSiblingIndex();
        transform.SetParent(transform.parent.parent, false);
        raycastImage.raycastTarget = false;
    }

    public void EnterInventory(Vector3 enterPosition)
    {
        playerController.dragObject.TurnOffDragObject();
        cardInfo.bgImage.gameObject.SetActive(true);
        transform.SetParent(inventoryTransform, false);
        transform.SetSiblingIndex(transformOrder);
		transform.position = enterPosition;
        Vector3 localPosition = transform.localPosition;
        localPosition.z = 0f;
        transform.localPosition = localPosition;
        raycastImage.raycastTarget = true;
		GameManager.Instance.player.inventory.UpdateAllCardCanvasSortingOrder();
	}

	public void SetCanvasSortingOrder(int order)
	{
		transformOrder = order;
		sortingCanvas.sortingOrder = SORTING_ORDER_IDLE * (transformOrder + 1);
		outlineSortingCanvas.sortingOrder = SORTING_ORDER_IDLE * (transformOrder + 1) + 1;
	}

	public void SetCanvasSortingOrder()
	{
		sortingCanvas.sortingOrder = SORTING_ORDER_IDLE * (transformOrder + 1);
		outlineSortingCanvas.sortingOrder = SORTING_ORDER_IDLE * (transformOrder + 1) + 1;
	}

	public void UpgradeCard(SOTrainCar _soTrainCar)
    {
        if (soTrainCar.nextSOTrainCar != null && soTrainCar.name.Contains(_soTrainCar.name))
        {
            int permanentHappiness = soTrainCar.permanentHappiness + _soTrainCar.permanentHappiness;
            InitCardData(soTrainCar.nextSOTrainCar.Clone());
            soTrainCar.permanentHappiness = permanentHappiness;
			AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.EventPopup);
			AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainDropMerge);
			EffectManager.Instance.PlayEffect(EffectManager.ECommonEffectType.cardUpgrade, transform.position + Vector3.up);
            // 차량칸
            if (playerController.selectedTrainCar != null &&
                playerController.selectedTrainCar.soTrainCar == _soTrainCar)
            {
                train.DeleteTrainCar(playerController.selectedTrainCar);
                playerController.selectedTrainCar = null;
            }
            // 차량카드
            else if (playerController.selectedCard != null &&
                playerController.selectedCard.soTrainCar == _soTrainCar)
            {
                GameManager.Instance.player.inventory.RemoveCard(playerController.selectedCard);
                playerController.selectedCard = null;
            }

            if (soTrainCar.name.Contains(AchievementData.ACHIEVEMENT_STRING_BABELTOWER))
            {
                SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_18);
            }
        }
    }

    public void MoveCard(PointerEventData eventData)
    {
        SOTrainCar _soTrainCar;
        InventoryCard curCard = null;
        Vector3 movePosition = playerController.dragObject.transform.position;

        if (playerController.selectedTrainCar != null)
        {
            _soTrainCar = playerController.selectedTrainCar.soTrainCar;
            InventoryCard inventoryCard = GameManager.Instance.player.inventory.AddCard(_soTrainCar);
            if (inventoryCard != null)
            {
                curCard = inventoryCard;
                train.DeleteTrainCar(playerController.selectedTrainCar);
                movePosition.x += 5f;
            }
            else
            {
                playerController.selectedTrainCar.BodySpriteTransparency(1f);
            }
        }
        else if (playerController.selectedCard != null)
        {
            curCard = playerController.selectedCard;
        }
        else
        {
            return;
        }

        if(curCard != null)
        {
            if (Camera.main.ScreenToWorldPoint(eventData.position).x < transform.position.x)
            {
                curCard.transformOrder = transform.GetSiblingIndex();
                transformOrder++;
            }
            else
            {
                curCard.transformOrder = transform.GetSiblingIndex() + 1;
            }

            curCard.EnterInventory(movePosition);
        }
        
        playerController.selectedTrainCar = null;
        playerController.selectedCard = null;
    }
    #endregion

    #region PrivateMethod
    private void InitMouseAction()
    {
        cardInfo.mouseAction = GetComponent<MouseAction>();
        cardInfo.mouseAction.onMouseDownLeft += MouseDown;
        cardInfo.mouseAction.onMouseUpLeft += MouseUp;
        cardInfo.mouseAction.onClickLeft += MouseClick;
        cardInfo.mouseAction.onMouseEnter = MouseEnter;
        cardInfo.mouseAction.onMouseOver = MouseOver;
        cardInfo.mouseAction.onMouseExit = MouseExit;
        cardInfo.mouseAction.onBeginDragLeft = BeginDrag;
        cardInfo.mouseAction.onDragLeft = Drag;
        cardInfo.mouseAction.onEndDragLeft = EndDrag;
        cardInfo.mouseAction.onDropLeft = Drop;
    }

    private void MouseDown(PointerEventData eventData)
    {
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        GameManager.Instance.player.train.HighlightUpgradableTrainCar(soTrainCar, true);
        GameManager.Instance.player.inventory.HighlightUpgradableCard(this, true);
    }

    private void MouseUp(PointerEventData eventData)
    {
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
    }

    private void MouseClick(PointerEventData eventData)
    {
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightSame(soTrainCar, true);
    }

    private void MouseEnter(PointerEventData eventData)
    {
        if(!eventData.dragging)
        {
            AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.CardChyack);
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightSame(soTrainCar, true);
			MouseEnterPerformance();
            cardInfo.ShowSynergyInfoList(soTrainCar);

            if(Camera.main.WorldToViewportPoint(transform.position).x < 0.5f)
            {
                cardInfo.UpdateSynergyInfoPosition(true, TextAnchor.LowerLeft);
            }
            else
            {
                cardInfo.UpdateSynergyInfoPosition(true, TextAnchor.LowerRight);
            }
        }
    }

    private void MouseOver()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform));
    }
    
    private void MouseExit(PointerEventData eventData)
    {
		MouseExitPerformance();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();

        if (!eventData.dragging)
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
		}
    }

    private void BeginDrag(PointerEventData eventData)
    {
        TrashCan.Instance.SetState(TrashCan.EState.Activated);
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();
        playerController.selectedCard = this;
        train.SetAllIntervalColliderEnabled(true);
        ExitInventory();
    }

    private void Drag(PointerEventData eventData)
    {
        if (playerController.dragObject.isActive == true && train.isMouseDragAndEnterInterval == false)
        {
            Vector3 pos = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)(MainCanvasController.Instance.transform), 
                eventData.position, GameManager.Instance.cameraController.overlayCamera, out pos);
            playerController.dragObject.transform.position = pos;
        }
    }

    private void EndDrag(PointerEventData eventData)
    {
        train.SetAllIntervalColliderEnabled(false);
        TrashCan.Instance.SetState(TrashCan.EState.Idle);

        if (train.isMouseDragAndEnterInterval == false && playerController.selectedCard != null)
        {
            //int lastCarIndex = train.GetCarLastIndex();
            TrainCar trainCar = train.AddTrainCar(playerController.selectedCard.soTrainCar);
            if (trainCar != null)
            {
                //train.InsertCar(trainCar, lastCarIndex);
                train.RepositionTrainCar(trainCar, trainCar);
                GameManager.Instance.player.inventory.RemoveCard(playerController.selectedCard);
                playerController.dragObject.TurnOffDragObject();
                MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
                MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
            }
            else
            {
                EnterInventory(playerController.dragObject.transform.position);
				NoticeManager.Instance.PrintMainNotice(GameManager.GetLocalizingString("공간이 부족합니다.", "You're out of space."));
			}

            playerController.selectedCard = null;
        }
    }

    private void Drop(PointerEventData eventData)
    {
        TrashCan.Instance.SetState(TrashCan.EState.Idle);
        DropCard(eventData);
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
    }

    private void DropCard(PointerEventData eventData)
    {
        SOTrainCar _soTrainCar;

        if (playerController.selectedTrainCar != null)
        {
            _soTrainCar = playerController.selectedTrainCar.soTrainCar;
        }
        else if (playerController.selectedCard != null)
        {
            _soTrainCar = playerController.selectedCard.soTrainCar;
        }
        else
        {
            return;
        }

        if(_soTrainCar.isSecurityTrainCar)
        {
			NoticeManager.Instance.PrintMainNotice(GameManager.GetLocalizingString("통제시설은 보관할 수 없습니다.", "You can't store control facilities."));
            playerController.selectedTrainCar.mouseAction.onEndDragLeft(eventData);
            return;
        }

        UpgradeCard(_soTrainCar);
        MoveCard(eventData);

        if (playerController.dragObject.isActive == true)
        {
            playerController.selectedTrainCar = null;
            playerController.selectedCard = null;
            playerController.dragObject.TurnOffDragObject();
            train.SetAllIntervalColliderEnabled(false);
            train.isMouseDragAndEnterInterval = false;
        }
    }

	private void MouseEnterPerformance()
	{
		AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.CardSsak);
		hoveringCard.transform.DOKill();
        hoveringCard.transform.localScale = Vector3.one * hoveringScale; //.DOScale(hoveringScale, 0f).SetUpdate(true);
		hoveringCard.transform.DOLocalMoveY(hoveringPosY, 0f).SetUpdate(true);
		sortingCanvas.sortingOrder = SORTING_ORDER_HOVER;
		outlineSortingCanvas.sortingOrder = SORTING_ORDER_HOVER + 1;
	}
	private void MouseExitPerformance()
	{
		hoveringCard.transform.DOKill();
        hoveringCard.transform.localScale = Vector3.one; //.DOScale(1f, 0f).SetUpdate(true);
		hoveringCard.transform.DOLocalMoveY(0, 0f).SetUpdate(true);
		SetCanvasSortingOrder();
	}
    #endregion
}
