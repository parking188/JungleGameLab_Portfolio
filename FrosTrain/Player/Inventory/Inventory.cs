using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	#region PublicVariables
	public GameObject inventoryCardPrefab;
	public MouseAction mouseAction;
	public int maxCapacity = 10;
	public List<InventoryCard> inventoryCards;
    public Action AddCardAction;
    public Action RemoveCardAction;
	#endregion

	#region PrivateVariables
	private TextMeshProUGUI countText;
    [SerializeField] private LerpHorizontalLayout lerpHorizontalLayout;
	[Header("Inventory Interval")]
	[SerializeField] private int intervalThresholdCount = 6;
	[SerializeField] private float intervalBase = -140f;
	[SerializeField] private float intervalMult = -10f;
    #endregion

    #region PublicMethod
    public InventoryCard AddCard(SOTrainCar soTrainCar, bool isRandomSelect = false)
	{
		if(inventoryCards.Count >= maxCapacity && !isRandomSelect)
		{
			//Debug.Log("Inventory is Full");
			NoticeManager.Instance.PrintMainNotice(GameManager.GetLocalizingString("공간이 부족합니다.", "You're out of space"));
			return null;
		}

		InventoryCard card = Instantiate(inventoryCardPrefab, transform).GetComponent<InventoryCard>();

        Vector3 position = transform.position;
        position.x += ((RectTransform)card.transform).rect.width;
        card.transform.position = position;

        card.InitInventoryCard();
		card.InitCardData(soTrainCar);
		inventoryCards.Add(card);
		UpdateAllCardCanvasSortingOrder();
		AddCardAction?.Invoke();
		UpdateCount();
        UpdateLerpHorizontalLayoutSpacing();

        if (isRandomSelect && inventoryCards.Count > maxCapacity)
        {
            OverInventoryCapacity();
        }

        return card;
	}

	public void UpdateAllCardCanvasSortingOrder()
	{
		for (int index = 0; index < inventoryCards.Count; index++)
		{
			InventoryCard card = inventoryCards[index];
			card.SetCanvasSortingOrder(card.transform.GetSiblingIndex());
		}
	}

    public void UpdateLerpHorizontalLayoutSpacing()
    {
        lerpHorizontalLayout.spacing = intervalBase + Math.Clamp(inventoryCards.Count - intervalThresholdCount, 0, int.MaxValue) * (intervalMult);
    }

	public void RemoveCard(InventoryCard card)
	{
		inventoryCards.Remove(card);

        DataManager.Instance.SaveInventory(inventoryCards);
        Destroy(card.gameObject);
        RemoveCardAction?.Invoke();
        UpdateCount();
        UpdateLerpHorizontalLayoutSpacing();
    }

	public void HighlightSynergyCard(ECarTag carTag, bool isOn = true)
	{
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            if (inventoryCards[index].soTrainCar.CompareCarTags(carTag))
            {
                inventoryCards[index].cardInfo.HighlightCard(isOn);
            }
        }
    }

    public void HighlightSameCard(SOTrainCar soTrainCar, bool isOn = true)
    {
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            if (inventoryCards[index].soTrainCar.title.Equals(soTrainCar.title))
            {
                inventoryCards[index].cardInfo.HighlightCard(isOn);
            }
        }
    }

    public void HighlightUpgradableCard(SOTrainCar soTrainCar, bool isOn = true)
    {
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            if (inventoryCards[index].soTrainCar.nextSOTrainCar != null &&
                inventoryCards[index].soTrainCar.title.Equals(soTrainCar.title) &&
                inventoryCards[index].soTrainCar.level == soTrainCar.level)
            {
                inventoryCards[index].cardInfo.HighlightCard(isOn);
            }
        }
    }

    public void HighlightUpgradableCard(InventoryCard card, bool isOn = true)
    {
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            if (inventoryCards[index] != card &&
                inventoryCards[index].soTrainCar.nextSOTrainCar != null &&
                inventoryCards[index].soTrainCar.title.Equals(card.soTrainCar.title) &&
                inventoryCards[index].soTrainCar.level == card.soTrainCar.level)
            {
                inventoryCards[index].cardInfo.HighlightCard(isOn);
            }
        }
    }

    public void HighlightUpgradableCardAuto()
    {
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            GameManager.Instance.player.train.HighlightUpgradableTrainCar(inventoryCards[index].soTrainCar);
            GameManager.Instance.player.inventory.HighlightUpgradableCard(inventoryCards[index]);
        }
    }

    public void OffAllHighlightCard()
    {
        for (int index = 0; index < inventoryCards.Count; index++)
        {
            inventoryCards[index].cardInfo.HighlightCard(false);
        }
    }
	#endregion

	#region PrivateMethod
	private void Awake()
	{
		transform.parent.Find("InventoryCount/InventoryCount_TMP").TryGetComponent(out countText);
	}
	private void Start()
    {
        GameManager.Instance.player.inventory = this;
		    mouseAction = transform.parent.GetComponent<MouseAction>();
        DataManager.Instance.LoadInventory();
        AddCardAction += MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight;
        AddCardAction += MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto;
		UpdateCount();
    }
	private void UpdateCount()
	{
		countText.text = (inventoryCards.Count > maxCapacity) ? "<color=#FF0000>" + inventoryCards.Count.ToString() + "/" + maxCapacity.ToString() + "</color>" : inventoryCards.Count.ToString() + "/" + maxCapacity.ToString();
	}

    private void OverInventoryCapacity()
    {
        GameManager.Instance.SetTimeControlPossibility(false);
        GameManager.Instance.TimeStop(true);
        GameManager.Instance.isInventoryFull = true;
        RemoveCardAction += CheckInventoryCapacity;
		NoticeManager.Instance.ShowSellText();
    }

    private void CheckInventoryCapacity()
    {
        if (inventoryCards.Count <= maxCapacity)
        {
            if (!GameManager.Instance.isTimeStoppedByEvent) GameManager.Instance.SetTimeControlPossibility(true);
            GameManager.Instance.isInventoryFull = false;
            if (!GameManager.Instance.isTimeStoppedByPlayer && !GameManager.Instance.isTimeStoppedByEvent) GameManager.Instance.TimeStop(false);
            RemoveCardAction -= CheckInventoryCapacity;
			NoticeManager.Instance.HideSellText();
        }
    }
    #endregion
}
