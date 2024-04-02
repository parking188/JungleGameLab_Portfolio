using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIRandomSelectPopup : MonoBehaviour
{
    #region PublicVariables
    public GameObject rewardItemPrefab;
    public GameObject contentArea;
    public Image background;
    public GameObject hideShowButton;
    public RectTransform droneTransitionUI;

    #endregion

    #region PrivateVariables
    private List<SORandomRewards> selectedRandomList = new List<SORandomRewards>();
	private List<RewardsAndChance> selectedRewards = new List<RewardsAndChance>();
	private List<GUIRandomSelectButton> rewardItemButtons = new List<GUIRandomSelectButton>();
	private bool isClosed = false;
	private bool isMoving = false;	
	private RectTransform targetPanel;
	private Image image;
	private CanvasGroup canvasGroup;
	private Sequence mySequence;
	[SerializeField] private GameObject hideButton;

	private const float CONTENT_AREA_SHOW_POS_Y = 400f;
	private const float CONTENT_AREA_HIDE_POS_Y = 1200f;
	[SerializeField] private float transitionTime = 0.8f;
	private TMP_Text HidsShowText;
    #endregion

    #region PublicMethod
    private void OnEnable()
    {
		background.gameObject.SetActive(true);
        DroneTransition();
    }

    private void OnDisable()
    {
        background.gameObject.SetActive(false);
        contentArea.gameObject.SetActive(false);
        hideShowButton.SetActive(false);
    }

    public void DroneTransition()
    {
        float widthTransitionOffset = Screen.width;
        float heighTtransitionOffset = Screen.height;

		AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.DroneTransition);
        Sequence sequence = DOTween.Sequence();
		sequence.Append(droneTransitionUI.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.OutBack, 0.6f))
			.Join(background.DOFade(0.5f, 0.5f))
            .Join(droneTransitionUI.transform.DOScale(Vector3.one * 5f, 0.6f))
            .AppendInterval(0.2f)
            .Append(droneTransitionUI.DOAnchorPos(new Vector2(-1600, 1200), 0.4f).SetEase(Ease.InBack))
            .AppendInterval(0.4f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                droneTransitionUI.anchoredPosition = new Vector2(3 * widthTransitionOffset, 1 * heighTtransitionOffset);
                droneTransitionUI.sizeDelta = Vector2.one;
				
				ShowPanel();

                //onCompleteAction?.Invoke();
            });
    }
    public void InitRandomSelectPopup(RewardSelect rewardSelect)
	{
		InitSelectedRewards(rewardSelect);
        //showPanel();
        //background.gameObject.SetActive(true);
        //hideButton.SetActive(true);
	}

	public void ResetRandomSelect()
    {
        int rewardItemButtonsCount = rewardItemButtons.Count;
        for (int index = 0; index < rewardItemButtonsCount; index++)
        {
			Destroy(rewardItemButtons[index].gameObject);
        }
        ClearingPanel();
    }

    public void ResetRandomSelectWithDelay()
	{
        StartCoroutine(IETempRandomSelectLateReset());
    }

    IEnumerator IETempRandomSelectLateReset()
    {
		hideButton.SetActive(false);
		int rewardItemButtonsCount = rewardItemButtons.Count;
		for (int index = 0; index < rewardItemButtonsCount; index++)
		{
			rewardItemButtons[index].button.enabled = false;
		}
		for (int index = 0; index < rewardItemButtonsCount; index++)
		{
			if (!rewardItemButtons[index].isSelected)
			{
				rewardItemButtons[index].DestoryUnselectedButton(transitionTime);
			}
		}
		yield return new WaitForSecondsRealtime(transitionTime * 1.3f + 0.2f);
		for (int index = 0; index < rewardItemButtonsCount; index++)
		{
			if (rewardItemButtons[index].isSelected)
			{
				rewardItemButtons[index].TransitionSelectedButton();
			}
		}
		yield return new WaitForSecondsRealtime(0.2f);
		background.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(ClearingPanel);
    }

	private void ClearingPanel()
	{
		((RectTransform)contentArea.transform).DOAnchorPosY(CONTENT_AREA_HIDE_POS_Y, 0f);
        rewardItemButtons.Clear();
        selectedRewards.Clear();
        selectedRandomList.Clear();
        gameObject.SetActive(false);
		if (!GameManager.Instance.isTimeStoppedByPlayer && !GameManager.Instance.isInventoryFull) GameManager.Instance.TimeStop(false);
        GameManager.Instance.isTimeStoppedByEvent = false;
        if (!GameManager.Instance.isInventoryFull) GameManager.Instance.SetTimeControlPossibility(true);
    }

	public void TogglePanel()
	{
		if (background.gameObject.activeSelf || contentArea.gameObject.activeSelf)
		{
			HidePanel();
		}
		else
		{
			ShowPanel();
		}
	}

	#endregion

	#region PrivateMethod
	private void Start()
    {
		image = GetComponent<Image>();
		targetPanel = contentArea.transform.parent.GetComponent<RectTransform>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        HidsShowText = hideShowButton.GetComponentInChildren<TMP_Text>();
    }
    /*
    private void ClosePanel()
	{
		if (!isMoving)
		{
            isMoving = true;
            Sequence closingSequence = DOTween.Sequence();
            turnButtonInteracting(false);
            closingSequence
            .Join(image.DOFade(0f, 0.2f))
            .Join(targetPanel.transform.Find("Btn_Hide").transform.DOScale(new Vector3(1, -1, 1), 0.5f))
			.Join(targetPanel.DOAnchorPosY(578, 1.0f).SetEase(Ease.OutExpo)).OnComplete(() =>
			{
                image.enabled = false; isClosed = true; isMoving = false;
                turnButtonInteracting(true);
            }).SetUpdate(true);
		}
	}

    private void OpenPanel()
	{
		if (!isMoving)
		{
            Sequence openingSequence = DOTween.Sequence();
			turnButtonInteracting(false);
            isMoving = false;
            image.enabled = true;
            openingSequence
                .Join(image.DOFade(0.5f, 0.2f))
                .Join(targetPanel.transform.Find("Btn_Hide").transform.DOScale(new Vector3(1, 1, 1), 0.5f))
                .Join(targetPanel.DOAnchorPosY(33, 1.0f).SetEase(Ease.OutExpo)).OnComplete(() =>
                {
                    isClosed = false; isMoving = false; turnButtonInteracting(true);
                }).SetUpdate(true);
        }
    }
	*/

    private void HidePanel()
	{
		if (mySequence.IsActive())
		{
			return;
		}
		for (int index = 0; index < rewardItemButtons.Count; index++)
		{
			rewardItemButtons[index].SetInteractablity(false);
		}
		mySequence = DOTween.Sequence();
		mySequence.Append(background.DOFade(0, 0.3f))
			.Join(((RectTransform)(contentArea.transform)).DOAnchorPosY(CONTENT_AREA_HIDE_POS_Y, 0.4f).SetEase(Ease.InBack))
			.SetUpdate(true)
			.OnComplete(() =>
			{
				background.gameObject.SetActive(false);
				contentArea.gameObject.SetActive(false);
			});
	}

	private void ShowPanel()
	{
		if (mySequence.IsActive())
		{
			return;
		}
		background.gameObject.SetActive(true);
		contentArea.gameObject.SetActive(true);
		hideButton.gameObject.SetActive(true);
		mySequence = DOTween.Sequence();
		mySequence.Append(background.DOFade(0.5f, 0.3f))
			.Join(((RectTransform)(contentArea.transform)).DOAnchorPosY(CONTENT_AREA_SHOW_POS_Y, 0.6f).SetEase(Ease.OutBack))
			.SetUpdate(true)
			.OnComplete(() =>
			{
				for (int index = 0; index < rewardItemButtons.Count; index++)
				{
					rewardItemButtons[index].SetInteractablity(true);
				}
			});
	}

	private void TurnButtonInteracting(bool value)
	{
		foreach(Transform child in contentArea.transform)
		{
			Button button;
			child.TryGetComponent(out button);
			button.interactable = value;
		}
	}
    private void InitSelectedRewards(RewardSelect rewardSelect)
    {
        for (int index = 0; index < rewardSelect.optionCount; index++)
		{
			if (rewardSelect.isTrainLevelBased)
			{
				rewardSelect.SelectTierRandom();
				if(rewardSelect.tiers.Count == 0)
				{
					Debug.Log("Error : No Tier In List");
					return;
				}
			}

            SelectRandomList(rewardSelect);
            if (selectedRandomList.Count <= 0)
            {
				ResetRandomSelect();
				break;
            }

            RewardsAndChance selectedReward = SelectRandomReward(rewardSelect);
			if(selectedReward != default)
			{
                selectedRewards.Add(selectedReward);
                InitSelectButton(selectedReward, rewardSelect.optionCount, index);
            }
        }

		if (selectedRewards.Count <= 0)
		{
			ResetRandomSelect();
		}
    }

	private void SelectRandomList(RewardSelect rewardSelect)
	{
		selectedRandomList.Clear();
		foreach(SORandomRewards soRandomRewards in GameEventManager.Instance.soRandomRewardsList)
		{
			if(soRandomRewards.CheckTiers(rewardSelect.tiers) &&
				soRandomRewards.CheckOptionTypes(rewardSelect.optionTypes))
			{
                selectedRandomList.Add(soRandomRewards.Clone());
            }
		}
    }

	private RewardsAndChance SelectRandomReward(RewardSelect rewardSelect)
	{
		float maxChance = 0f;

		foreach(SORandomRewards soRandomReward in selectedRandomList)
		{
			foreach(RewardsAndChance reward in soRandomReward.rewards)
			{
				if((reward.rewards[0] is RewardTrainCar && ((RewardTrainCar)(reward.rewards[0])).soTrainCar.CompareCarTags(rewardSelect.tags) == false) ||
					DuplicateCardCheck(reward))
				{
					reward.chance = 0f;
				}

                maxChance += reward.chance;
            }
		}

		float randomChance = UnityEngine.Random.Range(0f, maxChance);
		float currentChance = 0f;

        foreach (SORandomRewards soRandomReward in selectedRandomList)
        {
            foreach (RewardsAndChance reward in soRandomReward.rewards)
            {
				if(randomChance >= currentChance && randomChance < currentChance + reward.chance)
				{
					return reward;
				}
                currentChance += reward.chance;
            }
        }
		return default;
	}

	private void LayoutTemp(GUIRandomSelectButton button, int optionCount, int index)
	{
        RectTransform rectTransform = button.GetComponent<RectTransform>();

        float buttonWidth = rectTransform.rect.width;
        float buttonHeight = rectTransform.rect.height;
        // Define a maximum spacing value
        float maxSpacing = 160f;
        // Calculate the total width needed for all buttons with max spacing
        float totalWidthWithMaxSpacing = optionCount * buttonWidth + (optionCount - 1) * maxSpacing;
        // Calculate actual spacing based on screen width
        float actualSpacing;
        if (totalWidthWithMaxSpacing > Screen.width)
        {
            // Reduce spacing if total width exceeds screen width
            float availableWidthForSpacing = Screen.width - optionCount * buttonWidth;
            actualSpacing = availableWidthForSpacing / (optionCount - 1);
        }
        else
        {
            actualSpacing = maxSpacing;
        }
        actualSpacing = Mathf.Max(actualSpacing, 0); // Ensure spacing positive val
        // Calculate the start X position relative to the center
        float startX = -(optionCount * buttonWidth + (optionCount - 1) * actualSpacing) / 2 + buttonWidth / 2;
        // Calculate and set the position for this button
        float buttonX = startX + index * (buttonWidth + actualSpacing);
        rectTransform.anchoredPosition = new Vector2(buttonX, -buttonHeight / 4);

    }

    private void InitSelectButton(RewardsAndChance selectedReward, int optionCount, int index)
    {
		GUIRandomSelectButton button = Instantiate(rewardItemPrefab, contentArea.transform).GetComponent<GUIRandomSelectButton>();
		//===Temp Layout=========

		LayoutTemp(button, optionCount, index);

        //======================

        button.InitSelectButton();

        foreach (IReward reward in selectedReward.rewards)
        {
            reward.InitReward();
            button.button.onClick.AddListener(() => { reward.giveReward?.Invoke(); });
        }
		button.button.onClick.AddListener(() => { ResetRandomSelectWithDelay(); });

		//기차 다시 운행
		button.button.onClick.AddListener(() =>
		{
			//ExploreMapManager.Instance.exploreMapTrainMovement.CheckNextPath();
		});

		rewardItemButtons.Add(button);

		if (selectedReward.rewards[0] is RewardTrainCar)
		{
			button.InitSelectButtonData((selectedReward.rewards[0] as RewardTrainCar).soTrainCar);
		}
		else
		{
			button.cardInfo.UpdateNonTrainCarInfoData(selectedReward);
		}
	}

	private bool DuplicateCardCheck(RewardsAndChance reward)
	{
		for (int index = 0; index < selectedRewards.Count; index++)
		{
			if (selectedRewards[index].rewards[0] is RewardTrainCar)
			{
				SOTrainCar soTrainCar = ((RewardTrainCar)(selectedRewards[index].rewards[0])).soTrainCar;
				if (soTrainCar.title.Equals(((RewardTrainCar)reward.rewards[0]).soTrainCar.title))
				{
					return true;
				}
			}
			else
			{
                if (selectedRewards[index].title.Equals(reward.title))
                {
                    return true;
                }
            }
		}

		return false;
	}


    private void Update()
    {
		HidsShowText.text = GameManager.GetLocalizingString("감추기/보이기", "Hide/Show");
    }
    #endregion
}
