using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Spine.Unity;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine.Rendering.Universal.Internal;
using EPOOutline;
using UnityEngine.UIElements;

[RequireComponent(typeof(MouseAction))]
[Serializable]
public class TrainCar : MonoBehaviour, IConditionTarget, IRewardTarget
{
    #region PublicVariables
    public SOTrainCar soTrainCar;
    public CarContentBase carContentBase;
    public GameObject trainCarVisitStackPrefab;
    [HideInInspector] public TrainCarVisitStack trainCarVisitStack;
    public TrainCarInterval trainCarInterval;
    public TrainCarLight trainCarLight;
    public int carIndex;

    public Action ConditionCheckAction { get; set; }
    #endregion

    #region PrivateVariables
    public Train train { get; private set; }
    public GameObject bodyObj;
    private SpriteRenderer bodySprite;
    private SkeletonAnimation skeletonAnimation;
    private MeshRenderer meshRenderer;
    private BoxCollider2D carCollider;
    public MouseAction mouseAction { get; private set; }
    private PlayerController playerController;

    [SerializeField] private float bodyDragAlpha = 0.5f;
    public float hoveringTime = 0.1f;
    private float currentHoverTime;
    [HideInInspector] public bool isGetSynergy = false;

    private List<SOTask> taskList = new List<SOTask>();
    [SerializeField] private GameObject taskPrefab;
    public SOTask currentSOTask;
    private TaskController currentTaskObj;
    private float taskProb = 0.5f;
    private float taskMin = 30f;
    private float taskMax = 60f;

    [SerializeField] private Outlinable outline;
    [SerializeField] private SpriteRenderer iconSprite;
    #endregion

    #region PublicMethod
    public object GetConditionTarget(int index = 0)
    {
        switch(index)
        {
            case (int)ConditionTrainCar.ETrainCarCondition.Range:
                return soTrainCar.range;
            case (int)ConditionTrainCar.ETrainCarCondition.CurLaborer:
                return carContentBase.laborer.Value;
            case (int)ConditionTrainCar.ETrainCarCondition.MaxLaborer:
                return carContentBase.laborer.MaxValue;
            case (int)ConditionTrainCar.ETrainCarCondition.Tier:
                return soTrainCar.tier;
        }
        return null;
    }

    public void SetTrainCar(SOTrainCar _soTrainCar, int _carIndex, List<SOTask> taskList, List <float> taskSetting)
    {
        soTrainCar = _soTrainCar.Clone();
        carIndex = _carIndex;
        this.taskList = taskList;
        currentTaskObj = Instantiate(taskPrefab, transform).GetComponent<TaskController>();
        currentTaskObj.trainCar = this;
        currentTaskObj.gameObject.SetActive(false);
        if (taskSetting.Count != 0 && carIndex != 0)
        {
            taskProb = taskSetting[0];
            taskMin = taskSetting[1];
            taskMax = taskSetting[2]; 
            StartCoroutine(SpawnTaskPrefab());
        }
    }

    //public void LockOnThisCar()
    //{
    //    if (train.lockedOnTrainCar != null)
    //    {
    //        train.lockedOnTrainCar.CancelLockOn();
    //    }
    //
    //    vcam.Priority = 11;
    //    train.lockedOnTrainCar = this;
    //}

    //public void CancelLockOn()
    //{
    //    vcam.Priority = 9;
    //    train.lockedOnTrainCar = null;
    //}

    public bool CheckCarTags(ECarTag carTag)
    {
        for(int index = 0; index < soTrainCar.carTags.Count; index++)
        {
            if (soTrainCar.carTags[index] == carTag) return true;
        }

        return false;
    }

    public void AddReward(IReward reward)
    {
        soTrainCar.rewards.Add(reward);
    }

    public void AddRewards(List<IReward> rewards)
    {
        soTrainCar.rewards.AddRange(rewards);
    }

    public void RemoveRewards(List<IReward> rewards)
    {
        int rewardsCount = rewards.Count;
        for(int index = 0; index < rewardsCount; index++)
        {
            soTrainCar.rewards.Remove(rewards[index]);
        }
    }

	public void CancelRewards()
	{
		int rewardsCount = soTrainCar.rewards.Count;
		for (int index = 0; index < soTrainCar.rewards.Count; index++)
		{
			soTrainCar.rewards[index].cancelReward?.Invoke();
		}
	}

    public void CreateTrainCarVisitStack()
    {
        trainCarVisitStack = Instantiate(trainCarVisitStackPrefab, transform).GetComponent<TrainCarVisitStack>();
        Vector3 localPosition = Vector3.zero;
        localPosition.x += soTrainCar.carSize.x / 2.8f;
        localPosition.y += soTrainCar.carSize.y / 4f;
        trainCarVisitStack.transform.localPosition = localPosition;
        carContentBase.visitAction += trainCarVisitStack.ShowIncreaseStack;
    }

    public void InitTrainCarVisitStack(ConditionTrainCar conditionTrainCar)
    {
        if (trainCarVisitStack == null) CreateTrainCarVisitStack();
        trainCarVisitStack.InitTrainCarVisitStack(conditionTrainCar);
    }

    public void UpdateVisitStackText()
    {
        trainCarVisitStack.UpdateVisitStackText();
    }

    public void InitCar()
    {
        if(soTrainCar == null)
        {
            Debug.Log(gameObject.name + " SOData null Error");
            return;
        }

        //InitBodySprite-------------------------------------------------
        bodyObj = transform.Find("TrainCarBody").gameObject;
        if (soTrainCar.skeletonDataAsset == null)
        {
            bodySprite = bodyObj.AddComponent<SpriteRenderer>();
            bodySprite.sortingLayerName = "Object";
            bodySprite.sprite = soTrainCar.sprite;
            //bodyObj.transform.localScale = soTrainCar.carSize;
        }
        else
        {
            skeletonAnimation = SkeletonAnimation.AddToGameObject(bodyObj, soTrainCar.skeletonDataAsset);
            meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "Object";
            skeletonAnimation.state.SetAnimation(0, "animation", true);
        }
        //---------------------------------------------------------------

        carCollider = GetComponent<BoxCollider2D>();
        carCollider.size = soTrainCar.carSize;
        carCollider.offset = new Vector2(0f, (soTrainCar.carSize.y / 2f) - 1f);

        if (carIndex == 0)
        {
            transform.GetComponent<Animator>().enabled = false;
            bodyObj.transform.position += new Vector3(0f, 1.63f, 0f);
        }

        train = GetComponentInParent<Train>();
        playerController = PlayerController.Instance;

        //InitCarContentBase------------------------------------------------------
        carContentBase = (CarContentBase)gameObject.AddComponent(typeof(CarContentBase));
        carContentBase.InitCarContentBase(soTrainCar);
        for (int index = 0; index < soTrainCar.carSynergys.Count; index++)
        {
            soTrainCar.carSynergys[index].InitReward(this);
        }
        for (int index = 0; index < soTrainCar.rewards.Count; index++)
        {
            soTrainCar.rewards[index].InitReward(this);
        }
        for (int index = 0; index < soTrainCar.additionalRewards.Count; index++)
        {
            soTrainCar.additionalRewards[index].InitReward(this);
        }
        //-----------------------------------------------------------------------

        InitTrainCarLight(soTrainCar);
        InitOutline();
        InitIcon();
    }

    public void ChangeCar(SOTrainCar _soTrainCar)
    {
        CancelRewards();
        if (soTrainCar.skeletonDataAsset == null)
        {
            DestroyImmediate(bodySprite);
        }
        else
        {
            DestroyImmediate(skeletonAnimation);
        }
        DestroyImmediate(carContentBase);
        DestroyImmediate(soTrainCar);
        soTrainCar = _soTrainCar.Clone();
        InitCar();

        train.InitTrainTraits(this);
        train.GiveRewardTrainTraits();
        DataManager.Instance.SaveTrain(train.trainCars, train);
    }

    public void HighlightTrainCar(bool isOn)
    {
        if (isOn)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }

    public void ActiveIcon(bool isActive)
    {
        iconSprite.gameObject.SetActive(isActive);
    }

    public void BodySpriteTransparency(float alpha)
    {
        if (soTrainCar.skeletonDataAsset == null)
        {
            Color dragColor = bodySprite.color;
            dragColor.a = alpha;
            bodySprite.color = dragColor;
        }
        else
        {
            Color dragColor = meshRenderer.material.color;
            dragColor.a = alpha;
            meshRenderer.material.SetColor("_Color", dragColor);
        }
    }
    #endregion

    #region PrivateMethod
    private void Awake()
    {
        InitMouseAction();
        InitIntervalMouseAction();
    }

    private void InitMouseAction()
    {
        mouseAction = GetComponent<MouseAction>();
        mouseAction.onMouseDownLeft = MouseDown;
        mouseAction.onMouseUpLeft = MouseUp;
        mouseAction.onClickLeft = MouseClick;
        mouseAction.onBeginDragLeft = BeginDrag;
        mouseAction.onDragLeft = Drag;
        mouseAction.onEndDragLeft = EndDrag;
        mouseAction.onDropLeft = Drop;
        mouseAction.onMouseEnter = MouseEnter;
        mouseAction.onMouseExit = MouseExit;
        mouseAction.onMouseOver = MouseOver;
    }

    private void MouseDown(PointerEventData eventData)
    {
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        GameManager.Instance.player.train.HighlightUpgradableTrainCar(this, true);
        GameManager.Instance.player.inventory.HighlightUpgradableCard(soTrainCar, true);
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

    private void BeginDrag(PointerEventData eventData)
    {
        if(carIndex != 0)
        {
            train.ExitHoverTrainCarInfoUI();
            TrashCan.Instance?.SetState(TrashCan.EState.Activated);
            playerController.dragObject.InitDatas(soTrainCar);
            playerController.dragObject.TurnTrainCar();
            playerController.selectedTrainCar = this;
            BodySpriteTransparency(bodyDragAlpha);
            train.SetAllIntervalColliderEnabled(true);
        }
    }

    private void Drag(PointerEventData eventData)
    {
        if (playerController.dragObject.isActive == true && train.isMouseDragAndEnterInterval == false)
        {
            Vector3 mousePosition = eventData.position;
            mousePosition.z = playerController.dragObject.transform.position.z - Camera.main.transform.position.z;
            Vector3 screenPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            playerController.dragObject.transform.position = screenPosition;
		}
    }

    private void EndDrag(PointerEventData eventData)
    {
		TrashCan.Instance?.SetState(TrashCan.EState.Idle);
        if(playerController.dragObject.isActive == true)
        {
            BodySpriteTransparency(1f);
            playerController.dragObject.TurnOffDragObject();
            train.SetAllIntervalColliderEnabled(false);
            train.isMouseDragAndEnterInterval = false;
			playerController.selectedTrainCar = null;
		}

        if(eventData != null && eventData.pointerCurrentRaycast.gameObject == this.gameObject)
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightSame(soTrainCar, true);
        }
	}

    private void Drop(PointerEventData eventData)
    {
        TrashCan.Instance?.SetState(TrashCan.EState.Idle);
        UpgradeCar();
        SwapCar();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
    }

    private void MouseEnter(PointerEventData eventData)
    {
        if(!eventData.dragging)
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightSame(soTrainCar, true);
        }
        else
        {
            HighlightTrainCar(true);
        }
    }

    private void MouseExit(PointerEventData eventData)
    {
        currentHoverTime = 0f;
        GameManager.Instance.player.train.ExitHoverTrainCarInfoUI();

        if(!eventData.dragging)
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HighlightUpgradableAuto();
        }
        else
        {
            MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.OffAllHighlight();

            if (PlayerController.Instance.selectedTrainCar != null)
            {
                GameManager.Instance.player.train.HighlightUpgradableTrainCar(PlayerController.Instance.selectedTrainCar, true);
                GameManager.Instance.player.inventory.HighlightUpgradableCard(PlayerController.Instance.selectedTrainCar.soTrainCar, true);
            }
            else if (PlayerController.Instance.selectedCard != null)
            {
                GameManager.Instance.player.train.HighlightUpgradableTrainCar(PlayerController.Instance.selectedCard.soTrainCar, true);
                GameManager.Instance.player.inventory.HighlightUpgradableCard(PlayerController.Instance.selectedCard, true);
            }
        }
    }

    private void MouseOver()
    {
        if(playerController.selectedTrainCar != null || playerController.selectedCard != null)
        {
            return;
        }

        currentHoverTime += Time.unscaledDeltaTime;
        if (currentHoverTime >= hoveringTime)
        {
            train.UpdateHoverTrainCarInfoUI(carIndex);
        }
    }

    private void InitIntervalMouseAction()
    {
        if (trainCarInterval == null) trainCarInterval = GetComponentInChildren<TrainCarInterval>();
        trainCarInterval.boxCollider.enabled = false;
        trainCarInterval.mouseAction.onDropLeft = DropInterval;
        trainCarInterval.mouseAction.onMouseEnter = MouseEnterInterval;
        trainCarInterval.mouseAction.onMouseExit = MouseExitInterval;
    }

    private void InitTrainCarLight(SOTrainCar soTrainCar)
    {
        if (trainCarLight == null) trainCarLight = GetComponentInChildren<TrainCarLight>();
        if (soTrainCar.skeletonDataAsset == null)
        {
            trainCarLight.spriteLight.lightCookieSprite = soTrainCar.sprite;
        }
        else
        {
            // skeletonDataAsset이 있는 경우
        }
    }

    private void InitOutline()
    {
        if (outline == null) outline = GetComponentInChildren<Outlinable>();
        outline.AddAllChildRenderersToRenderingList();
    }

    private void InitIcon()
    {
        if (iconSprite == null) iconSprite = transform.Find("IconSprite").GetComponent<SpriteRenderer>();
        if (soTrainCar.carTags.Count > 0)
        {
            iconSprite.sprite = MainCanvasController.Instance.guiSynergyPanel.soTrainSynergyTexts[(int)soTrainCar.carTags[0]].carIcon;
        }
    }

    private void DropInterval(PointerEventData eventData)
    {
        TrashCan.Instance?.SetState(TrashCan.EState.Idle);
        InsertCar();
        train.isMouseDragAndEnterInterval = false;
    }

    private void MouseEnterInterval(PointerEventData eventData)
    {
		if(playerController.dragObject.isActive == true)
		{
			// 차량칸을 들고 있는 경우
			if (playerController.selectedTrainCar != null &&
				playerController.selectedTrainCar.carIndex != carIndex && playerController.selectedTrainCar.carIndex != carIndex + 1)
			{
				train.isMouseDragAndEnterInterval = true;
				playerController.dragObject.transform.position = (Vector2)(trainCarInterval.transform.position) + trainCarInterval.boxCollider.offset;
				AudioManager.Instance.PlaySFX(AudioManager.ESoundType.TrainGacha);
			}
			// 차량카드를 들고 있는 경우
			else if (playerController.selectedCard != null)
			{
				train.isMouseDragAndEnterInterval = true;
				Vector3 pos = Vector3.zero;
				RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)(MainCanvasController.Instance.transform),
					Camera.main.WorldToScreenPoint((Vector2)(trainCarInterval.transform.position) + trainCarInterval.boxCollider.offset),
					GameManager.Instance.cameraController.overlayCamera, out pos);
				playerController.dragObject.transform.position = pos;
				AudioManager.Instance.PlaySFX(AudioManager.ESoundType.TrainGacha);
			}
		}
    }

    private void MouseExitInterval(PointerEventData eventData)
    {
        if (playerController.dragObject.isActive == true)
        {
            train.isMouseDragAndEnterInterval = false;

			if ((playerController.selectedTrainCar != null && playerController.selectedTrainCar.carIndex != carIndex && playerController.selectedTrainCar.carIndex != carIndex + 1) ||
				(playerController.selectedCard != null))
			{
				//AudioManager.Instance.PlaySFX(AudioManager.ESoundType.TrainDropSnap);
			}
		}
    }

    private void UpgradeCar()
    {
        SOTrainCar _soTrainCar;

        if (playerController.selectedTrainCar != null)
        {
            _soTrainCar = playerController.selectedTrainCar.soTrainCar;
            playerController.selectedTrainCar.BodySpriteTransparency(1f);
        }
        else if (playerController.selectedCard != null)
        {
            _soTrainCar = playerController.selectedCard.soTrainCar;
        }
        else
        {
            return;
        }

        if (soTrainCar.nextSOTrainCar != null && soTrainCar.name.Contains(_soTrainCar.name))
        {
            AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.EventPopup);
            AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainDropMerge);
            EffectManager.Instance.PlayEffect(EffectManager.ECommonEffectType.trainCarUpgrade, transform.position);
            carContentBase.CycleCarContentBase();
            int permanentHappiness = soTrainCar.permanentHappiness + _soTrainCar.permanentHappiness;
            ChangeCar(soTrainCar.nextSOTrainCar);
            soTrainCar.permanentHappiness = permanentHappiness;

            // 차량칸
            if (playerController.selectedTrainCar != null)
            {
                train.DeleteTrainCar(playerController.selectedTrainCar);
                playerController.selectedTrainCar = null;
            }
            // 차량카드
            else if (playerController.selectedCard != null)
            {
                GameManager.Instance.player.inventory.RemoveCard(playerController.selectedCard);
                playerController.selectedCard = null;
            }

            if (playerController.dragObject.isActive == true)
            {
                playerController.selectedTrainCar = null;
                playerController.dragObject.TurnOffDragObject();
                train.SetAllIntervalColliderEnabled(false);
                train.isMouseDragAndEnterInterval = false;
            }

            if (soTrainCar.name.Contains(AchievementData.ACHIEVEMENT_STRING_BABELTOWER))
            {
                SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_18);
            }
        }
    }

    private void SwapCar()
    {
        if (carIndex != 0)
        {
            if (playerController.selectedTrainCar != null)
            {
                train.SwapCar(this, playerController.selectedTrainCar);
                playerController.selectedTrainCar.BodySpriteTransparency(1f);
                PlayerController.Instance.selectedTrainCar = null;
            }

            if (playerController.selectedCard != null && !soTrainCar.isSecurityTrainCar)
            {
                if (train.SwapCar(this, playerController.selectedCard)) PlayerController.Instance.selectedCard = null;
            }
        }
        else
        {
            Debug.Log("Can't Swap Engine Car");
        }

        if (playerController.dragObject.isActive == true)
        {
            playerController.selectedTrainCar = null;
            playerController.dragObject.TurnOffDragObject();
            train.SetAllIntervalColliderEnabled(false);
            train.isMouseDragAndEnterInterval = false;
        }
    }

    private void InsertCar()
    {
        // 차량칸
        if(playerController.selectedTrainCar != null)
        {
            if(playerController.selectedTrainCar.carIndex <= carIndex)
            {
                train.InsertCar(playerController.selectedTrainCar, carIndex);
            }
            else
            {
                int index = (carIndex < train.GetCarLastIndex()) ? carIndex + 1 : carIndex;
                train.InsertCar(playerController.selectedTrainCar, index);
            }
        }
        // 차량카드
        else if(playerController.selectedCard != null)
        {
            TrainCar newTrainCar = train.AddTrainCar(playerController.selectedCard.soTrainCar);
            if (newTrainCar != null)
            {
                int index = (carIndex < newTrainCar.carIndex) ? carIndex + 1 : carIndex;
                train.InsertCar(newTrainCar, index);
                if (index == newTrainCar.carIndex) train.RepositionTrainCar(newTrainCar, newTrainCar);

                GameManager.Instance.player.inventory.RemoveCard(playerController.selectedCard);
                playerController.dragObject.TurnOffDragObject();
            }
            else
            {
                playerController.selectedCard.EnterInventory(playerController.dragObject.transform.position);
				NoticeManager.Instance.PrintMainNotice(GameManager.GetLocalizingString("공간이 부족합니다.", "You're out of space."));
			}
            
            playerController.selectedCard = null;
        }
    }

    private IEnumerator SpawnTaskPrefab()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(taskMin, taskMax);
            yield return new WaitForSeconds(waitTime);

            if (UnityEngine.Random.value < taskProb)
            {
                currentSOTask = taskList[UnityEngine.Random.Range(0, taskList.Count-1)];
                currentTaskObj.ShowTask(currentSOTask.taskSprite, currentSOTask.lastingTime);       
            }
        }
    }
    #endregion
}