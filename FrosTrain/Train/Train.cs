using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MonoBehaviour, IConditionTarget, IRewardTarget
{
    #region PublicVariables
    public List<TrainCar> GetCarList() => trainCars;
    public int GetCarLastIndex() => trainCars.Count - 1;
    //[HideInInspector] public TrainCar lockedOnTrainCar;
    public List<SOTrainCar> initSOTrainCars;
    [SerializeField] private List<SOTrainSynergy> initTrainSynergies = new List<SOTrainSynergy>();
    [SerializeField] private List<SOTrainTrait> initTrainTraits = new List<SOTrainTrait>();
    public GUITrainCarHoverInfo trainCarHoverInfo;

    [HideInInspector] public bool isMouseDragAndEnterInterval = false;

    [HideInInspector] public int currentLaborerCount = 0;
    [HideInInspector] public Queue<CarContentBase> laborerMoveCarOrder = new Queue<CarContentBase>();

    [HideInInspector] public List<int> carTagCountList;

    [HideInInspector] public List<SOTrainTrait> soTrainTraits = new List<SOTrainTrait>();
    [HideInInspector] public List<SOTrainSynergy> soTrainSynergies = new List<SOTrainSynergy>();
    public Action ConditionCheckAction { get; set; }

    [HideInInspector] public Action AddOrDeleteTrainCarAction;
    [HideInInspector] public Action InitLaborVariablesAction;
    [HideInInspector] public Action MoveLaborerRandomCarAction;
    [HideInInspector] public Action LaborSynergyAction;
    [HideInInspector] public Action MoveLaborerRandomCarAfterAction;
    [HideInInspector] public Action LaborSynergyAfterAction;
    [HideInInspector] public Action LaborAction;
    [HideInInspector] public Action LaborAfterAction;

    [HideInInspector] public TrainCar securityTrainCar;
	#endregion

	#region PrivateVariables
    private CameraController cameraController;
	[SerializeField] private CameraShaker shaker;
    private float currentLaborerMoveTime = 0f;
    private float delayTimeHappinessBomb = 0f;
    private float currentDelayTimeHappinessBomb = 0f;
    [HideInInspector] public int laboredTrainCarCount = 0;

    [Header("Labor")]
    public float laborCycleTime = 10f;
    public float laborCycleTimeWeightSynergy = 0f;
    public float laborCycleTimeWeightTrainCar = 0f;
    [SerializeField] private float totalLaborerMoveTimeRate = 0.3f;
    [SerializeField] private float startDelayTimeHappinessBomb = 0.5f;
    [SerializeField] private float endDelayTimeHappinessBomb = 0.5f;
    [SerializeField] private float maxDelayTimeHappinessBomb = 0.5f;
    private Coroutine laborCoroutine;
    public float CurLaborCycleTime => Mathf.Clamp(laborCycleTime - laborCycleTimeWeightSynergy - laborCycleTimeWeightTrainCar, 1f, 10f);

    [Header("TrainCar")]
    [SerializeField] private GameObject trainCarPrefab;
    public int maxCapacityTrainCars;
    public int curCapacityTrainCars;
    public List<TrainCar> trainCars;

    [Header("Train")]
    private float defaultTrainSpeed;
    private float trainSpeed;

    private List<SOTask> taskList = new List<SOTask>();
    [Header("Task")]
    [SerializeField] private float taskProb = 0.5f;
    [SerializeField] private float taskMin = 30f;
    [SerializeField] private float taskMax = 60f;

    [Header("SelectCard")]
    public int selectCardOptionCount = 3;
    [SerializeField] private List<tierChancesStruct> originalTierChanceByLevel;
    [HideInInspector] public List<tierChancesStruct> tierChanceByLevel = new List<tierChancesStruct>();


    public float TrainSpeed => trainSpeed;

    public int level { get; private set; }
    public float curExp { get; private set; }
    public List<float> requireExpList = new List<float>();

    public int engineLevel { get; private set; }
    public float curEngineExp { get; private set; }
    public List<float> requireEngineExpList = new List<float>();
    public List<float> speedRateByEngineLevelList = new List<float>();
    public int engineExpExceedHappiness = 100;

    public float curTotalTraining = 0f;
    public int needTrainingEvaluation = 5;
	[HideInInspector] public int previousRedFrostTemplarLevel = 0;
    [HideInInspector] public int trainingEvaluationCount;
    [HideInInspector] public float trainingMultipleBySynergy = 0f;
    [HideInInspector] public float trainingMultipleByTrainCar = 0f;
    public float TotalTrainingMultiple => trainingMultipleBySynergy + trainingMultipleByTrainCar;

    public int academyCycle = 3;
    [HideInInspector] public int previousAcademyLevel = 0;
    public int recordedArea = 0;

    public List<guildOfExplorerSynergyData> guildOfExplorersSynergyTable = new List<guildOfExplorerSynergyData>();

	//TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	[SerializeField] private TestTrainShake test;
	public float totalHappiness;
	//TESTEND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	private Dictionary<string, List<TrainCar>> carTypeDic = new Dictionary<string, List<TrainCar>>();
    private bool isTrainIconOn = true;
    public bool isBeforeInit { get; private set; } = true;
	#endregion

	#region PublicMethod 
	//public void CameraLockOn(TrainCar trainCar)
	//{
	//    if (lockedOnTrainCar != null)
	//    {
	//        lockedOnTrainCar.CancelLockOn();
	//    }
	//
	//    trainCar.LockOnThisCar();
	//    lockedOnTrainCar = trainCar;
	//}

	//public void CameraLockOff()
	//{
	//    if (lockedOnTrainCar != null)
	//    {
	//        lockedOnTrainCar.CancelLockOn();
	//        lockedOnTrainCar = null;
	//    }
	//}

	#region TrainCarBehaviour
	[Button]
    public TrainCar AddTrainCar(SOTrainCar soTrainCar)
    {
        if (curCapacityTrainCars >= maxCapacityTrainCars && !soTrainCar.isSecurityTrainCar) return null;
        else if (soTrainCar.CompareCarTags(ECarTag.Large) && (curCapacityTrainCars + 1 >= maxCapacityTrainCars)) return null;

        int trainIndex = trainCars.Count;
        if (!soTrainCar.isSecurityTrainCar) curCapacityTrainCars++;
        if (soTrainCar.CompareCarTags(ECarTag.Large)) curCapacityTrainCars++;
        GameObject trainCarObj = Instantiate(trainCarPrefab, transform.position, trainCarPrefab.transform.rotation, transform);
        TrainCar trainCar = trainCarObj.GetComponent<TrainCar>();
        trainCarObj.name = "TrainCar " + trainIndex.ToString();
        List<float> taskSetting = new List<float>();
        taskSetting = new List<float> { taskProb, taskMin, taskMax };
        trainCar.SetTrainCar(soTrainCar, trainIndex, taskList, taskSetting);
        trainCar.InitCar();
        trainCar.ActiveIcon(isTrainIconOn);
        trainCars.Add(trainCarObj.GetComponent<TrainCar>());
        CountCarTags(trainCar, false);

        InitTrainTraits(trainCar);
        GiveRewardTrainTraits();
        if (soTrainCar.isSecurityTrainCar == false) AddOrDeleteTrainCarAction?.Invoke();
        else securityTrainCar = trainCar;
		//TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		test.AddModules(trainCar);
		//TESTEND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		SpareSpaceIndicator.Instance.UpdateSpareSpace(curCapacityTrainCars - 1, maxCapacityTrainCars - 1);
		ExploreManager.Instance.guiLevelPanel.UpdateCapacityTrainCar();
        DataManager.Instance.SaveTrain(trainCars, this);
		return trainCar;
    }

    public void DeleteTrainCar(TrainCar trainCar)
    {
        trainCar.carContentBase.CycleCarContentBase();
        trainCar.transform.DOKill();
        int trainIndex = trainCar.carIndex;
		if (!trainCar.soTrainCar.isSecurityTrainCar) curCapacityTrainCars--;
		else securityTrainCar = null;
		if (trainCar.soTrainCar.CompareCarTags(ECarTag.Large)) curCapacityTrainCars--;
		CountCarTags(trainCar, true);
		//TEST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		test.RemoveModules(trainCar);
		//TESTEND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		trainCar.CancelRewards();
		trainCars.Remove(trainCar);
        Destroy(trainCar.gameObject);
		ReorderCarIndex();
        if (trainIndex < trainCars.Count) RepositionTrainCar(trainCars[trainIndex]);
        if (trainCar.soTrainCar.isSecurityTrainCar == false) AddOrDeleteTrainCarAction?.Invoke();
		SpareSpaceIndicator.Instance.UpdateSpareSpace(curCapacityTrainCars - 1, maxCapacityTrainCars - 1);
		ExploreManager.Instance.guiLevelPanel.UpdateCapacityTrainCar();
        DataManager.Instance.SaveTrain(trainCars, this);
	}

    public void SwapCar(TrainCar car1, TrainCar car2)
    {
        trainCars.SwapList(car1.carIndex, car2.carIndex);
        car1.carIndex.Swap(ref car2.carIndex);

        if (car1.carIndex < car2.carIndex)
        {
            RepositionTrainCar(car1, car1, car2);
        }
        else
        {
            RepositionTrainCar(car2, car2, car1);
        }
    }

    public bool SwapCar(TrainCar trainCar, InventoryCard card)
    {
        if (trainCar.soTrainCar.isSecurityTrainCar)
        {
            return false;
        }

        if(card.soTrainCar.CompareCarTags(ECarTag.Large))
        {
            if (trainCar.soTrainCar.CompareCarTags(ECarTag.Large) == false && curCapacityTrainCars >= maxCapacityTrainCars)
            {
                Debug.Log("Can't Swap!!!");
				NoticeManager.Instance.PrintMainNotice(GameManager.GetLocalizingString("공간이 부족합니다.", "You're out of space."));
                return false;
            }
        }

        int carIndex = trainCar.carIndex;
        SOTrainCar soTrainCar = trainCar.soTrainCar;
        DeleteTrainCar(trainCar);
        TrainCar newTrainCar = AddTrainCar(card.soTrainCar);
        if (newTrainCar != null)
        {
            InsertCar(newTrainCar, carIndex, true);
            card.InitCardData(soTrainCar);
            card.EnterInventory(card.transform.position);
            RepositionTrainCar(trainCars[1], newTrainCar);
        }

        return true;
    }

    public void ReorderCarIndex()
    {
        for(int index = 0; index < trainCars.Count; index++)
        {
            trainCars[index].carIndex = index;
        }
    }

    public void RepositionTrainCar(TrainCar trainCar, TrainCar droppedCar = null, TrainCar droppedCar2 = null)
    {
        if(trainCar.carIndex == 0)
        {
            Debug.Log("Can't Reposition Engine Car");
            return;
        }
		Vector3 reposition = trainCars[trainCar.carIndex - 1].transform.position;
        reposition.y = trainCars[0].transform.position.y;

        for(int index = trainCar.carIndex; index < trainCars.Count; index++)
        {
            float trainInterval = (trainCars[index - 1].soTrainCar.carSize.x / 2f) + (trainCars[index].soTrainCar.carSize.x / 2f)
                + (trainCars[index - 1].soTrainCar.carInterval / 2f) + (trainCars[index].soTrainCar.carInterval / 2f);
            reposition -= new Vector3(trainInterval, 0f, 0f);

			TrainCar current = trainCars[index];
			if (trainCars[index] == droppedCar ||
                trainCars[index] == droppedCar2)
			{
				current.transform.DOKill();
				current.transform.DOMove(reposition, 0.3f)
					.From(reposition + Vector3.up)
					.SetUpdate(true).SetEase(Ease.InCirc, 5)
					.OnComplete(() =>
					{
						AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainDrop);
						AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainDropSnap);
						shaker.TrainCarDropShake();
						EffectManager.Instance.PlayOneShot(EffectManager.EParticleEffectType.trainCarChange, current.transform.position);
						current.carContentBase?.RefreshLaborerStackPosition();
					} );
			}
			else
			{
				current.transform.DOKill();
				current.transform.DOMove(reposition, 0.4f)
					.SetUpdate(true)
					.OnComplete(() =>
					{
						current.carContentBase?.RefreshLaborerStackPosition();
					});
			}
        }

        SetIntervalTrainCar(trainCar);
        cameraController.cameraMinXOffset = reposition.x - 5f;
        Physics2D.SyncTransforms();
        DataManager.Instance.SaveTrain(trainCars, this);
    }

    public void SetIntervalTrainCar(TrainCar trainCar)
    {
        for (int index = trainCar.carIndex; index < trainCars.Count; index++)
        {
            Vector2 offset = Vector2.zero;
            offset.x -= (trainCars[index - 1].soTrainCar.carSize.x + trainCars[index - 1].soTrainCar.carInterval) / 2f;
            trainCars[index - 1].trainCarInterval.boxCollider.offset = offset;

            Vector2 size = Vector2.zero;
            size.x = (trainCars[index - 1].soTrainCar.carInterval + trainCars[index].soTrainCar.carInterval) * 3f;
            size.y = trainCars[index - 1].soTrainCar.carSize.y * 2f;
            trainCars[index - 1].trainCarInterval.boxCollider.size = size;
        }

        int lastIndex = trainCars.Count - 1;
        Vector2 lastOffset = Vector2.zero;
        lastOffset.x -= (trainCars[lastIndex].soTrainCar.carSize.x + trainCars[lastIndex].soTrainCar.carInterval) / 2f;
        trainCars[lastIndex].trainCarInterval.boxCollider.offset = lastOffset;

        Vector2 lastSize = Vector2.zero;
        lastSize.x = trainCars[lastIndex].soTrainCar.carInterval * 6f;
        lastSize.y = trainCars[lastIndex].soTrainCar.carSize.y * 2f;
        trainCars[lastIndex].trainCarInterval.boxCollider.size = lastSize;
    }

    public void SetAllIntervalColliderEnabled(bool isIntervalEnabled)
    {
        for(int index = 0; index < trainCars.Count; index++)
        {
            trainCars[index].trainCarInterval.boxCollider.enabled = isIntervalEnabled;
        }
    }

    public void InsertCar(TrainCar trainCar, int insertIndex, bool isSwap = false)
    {
        if (trainCar.carIndex == 0 || insertIndex == 0)
        {
            Debug.Log("Can't insert Engine Car");
            return;
        }

        if(trainCar.carIndex == insertIndex)
        {
            if(isSwap) RepositionTrainCar(trainCar, trainCar);
            return;
        }

        int repositionIndex = (trainCar.carIndex > insertIndex) ? insertIndex : trainCar.carIndex;
        trainCars.MoveTo(trainCar.carIndex, insertIndex);
        RepositionTrainCar(trainCars[repositionIndex], trainCar);
    }

    public void ToggleAllIcon()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            trainCars[index].ActiveIcon(!isTrainIconOn);
        }
        isTrainIconOn = !isTrainIconOn;
    }
    #endregion

    #region LaborPipeline
    private void StartLaborTime(float newLaborTime)
    {
        if(laborCoroutine != null)
        {
            StopCoroutine(laborCoroutine);
        }

        laborCycleTime = newLaborTime;
        laborCoroutine = StartCoroutine(StartLabor());
    }

    private IEnumerator StartLabor()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            InitLaborVariables();
            MoveLaborerRandomCar();
            LaborSynergy();
            MoveLaborerRandomCarAfter();
            LaborSynergyAfter();
            Labor();
            LaborAfter();
            yield return new WaitForSeconds(CurLaborCycleTime);
        }
    }

    private void InitLaborVariables()
    {
        currentLaborerMoveTime = 0f;
        delayTimeHappinessBomb = 0f;
        currentDelayTimeHappinessBomb = 0f;
        laboredTrainCarCount = 0;

        currentLaborerCount = 0;
        for (int index = 0; index < trainCars.Count; index++)
        {
            trainCars[index].carContentBase.OutAllLaborer();
            trainCars[index].trainCarVisitStack?.ResetGainedVisit();
        }

        InitLaborVariablesAction?.Invoke();
    }

    private void MoveLaborerRandomCar()
    {
        for (int laborer = 0; laborer < ResourceManager.Instance.GetPopulation().Value; laborer++)
        {
            int randomCarIndex = 0;
            do
            {
                randomCarIndex = UnityEngine.Random.Range(1, trainCars.Count);
            }
            while (!CheckAllCarIsLaborerFull() && !trainCars[randomCarIndex].carContentBase.TryAddOneLaborer());
        }

        MoveLaborerRandomCarAction?.Invoke();
    }

    private void LaborSynergy()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].carContentBase.CheckTrainCarLabor())
            {
                trainCars[index].carContentBase.SynergyTrainCar();
            }
        }

        LaborSynergyAction?.Invoke();
    }

    private void MoveLaborerRandomCarAfter()
    {
        for (int index = 0; index < currentLaborerCount; index++)            
        {
            CarContentBase carContentBase = laborerMoveCarOrder.Dequeue();
            carContentBase.StartCoroutine(carContentBase.CallEffectLaborerStack(currentLaborerMoveTime));

            // Linear vs Lerping ---> Linear
            currentLaborerMoveTime += (CurLaborCycleTime * totalLaborerMoveTimeRate) / currentLaborerCount;
        }

        MoveLaborerRandomCarAfterAction?.Invoke();
    }

    private void LaborSynergyAfter()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].carContentBase.CheckTrainCarLabor())
            {
                laboredTrainCarCount += trainCars[index].carContentBase.laborCount;
                trainCars[index].carContentBase.SynergyAfterTrainCar();
            }
        }

        LaborSynergyAfterAction?.Invoke();

        delayTimeHappinessBomb = (CurLaborCycleTime - (CurLaborCycleTime * totalLaborerMoveTimeRate) - startDelayTimeHappinessBomb - endDelayTimeHappinessBomb) / laboredTrainCarCount;
        delayTimeHappinessBomb = (delayTimeHappinessBomb <= maxDelayTimeHappinessBomb) ? delayTimeHappinessBomb : maxDelayTimeHappinessBomb;
        currentDelayTimeHappinessBomb = (CurLaborCycleTime * totalLaborerMoveTimeRate) + startDelayTimeHappinessBomb - delayTimeHappinessBomb;
    }

    private void Labor()
    {
        for(int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].carContentBase.CheckTrainCarLabor())
            {
                trainCars[index].carContentBase.LaborTrainCar();
                trainCars[index].carContentBase.StartCoroutine(DelayedGainHappiness(trainCars[index]));
            }
        }

        LaborAction?.Invoke();
    }

    private void LaborAfter()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].carContentBase.CheckTrainCarLabor())
            {
                trainCars[index].carContentBase.SynergyCancelTrainCar();
            }
        }

        LaborAfterAction?.Invoke();
    }

    private bool CheckAllCarIsLaborerFull()
    {
        bool isAllCarLaborerFull = true;
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].carContentBase.isLaborerFull == false)
            {
                isAllCarLaborerFull = false;
                break;
            }
        }

        return isAllCarLaborerFull;
    }

    private IEnumerator DelayedGainHappiness(TrainCar trainCar)
    {
        currentDelayTimeHappinessBomb += delayTimeHappinessBomb;
        yield return new WaitForSeconds(currentDelayTimeHappinessBomb);
        trainCar.carContentBase.CycleCarContentBase();
    }

    public IEnumerator DelayedGainHappiness(TrainCar trainCar, int gainedHappiness, Vector3 position, Vector3 textPosFromPosition)
    {
        currentDelayTimeHappinessBomb += delayTimeHappinessBomb;
        yield return new WaitForSeconds(currentDelayTimeHappinessBomb);
        trainCar.carContentBase.GainHappiness(gainedHappiness, position, textPosFromPosition);
    }

    public IEnumerator DelayedGainHappinessForTraining(TrainCar trainCar, int gainedHappiness, Vector3 position, Vector3 textPosFromPosition, float maintain)
    {
        currentDelayTimeHappinessBomb += delayTimeHappinessBomb;
        yield return new WaitForSeconds(currentDelayTimeHappinessBomb);
        trainCar.carContentBase.GainHappiness(gainedHappiness, position, textPosFromPosition);

        curTotalTraining *= maintain;
        DataManager.Instance.SaveCurTotalTrainingData(trainCar.train.curTotalTraining);
        HiddenStatManager.Instance.Training.UpdateAmount((int)curTotalTraining);
        HiddenStatManager.Instance.Training.UpdateTurn(trainingEvaluationCount);
        DataManager.Instance.SaveTrainingEvaluationCountData(trainingEvaluationCount);
    }

    public IEnumerator ImmediatelyGainHappiness(TrainCar trainCar, int gainedHappiness, Vector3 position, Vector3 textPosFromPosition)
    {
        yield return new WaitForSeconds(currentDelayTimeHappinessBomb);
        trainCar.carContentBase.GainHappiness(gainedHappiness, position, textPosFromPosition);
    }

    public void CheckIsSynergyInRange(TrainCar trainCar)
    {
        if(trainCar.soTrainCar.range > 0)
        {
            for(int index = 1; index <= trainCar.soTrainCar.range; index++)
            {
                if(trainCar.carIndex + index < trainCars.Count && trainCars[trainCar.carIndex + index].isGetSynergy)
                {
                    trainCars[trainCar.carIndex + index].UpdateVisitStackText();
                    trainCars[trainCar.carIndex + index].isGetSynergy = false;
                }

                if(trainCar.carIndex - index > 0 && trainCars[trainCar.carIndex - index].isGetSynergy)
                {
                    trainCars[trainCar.carIndex - index].UpdateVisitStackText();
                    trainCars[trainCar.carIndex - index].isGetSynergy = false;
                }
            }
        }
    }

    public void CountLaborerMove(CarContentBase carContentBase)
    {
        laborerMoveCarOrder.Enqueue(carContentBase);
        currentLaborerCount++;
    }

    public void UpdateHoverTrainCarInfoUI(int carIndex)
    {
        trainCarHoverInfo.cardInfo.UpdateIfNullGUICardInfoData(trainCars[carIndex].soTrainCar);
        trainCarHoverInfo.UpdateCardPosition(trainCars[carIndex]);
    }

    public void ExitHoverTrainCarInfoUI()
    {
        trainCarHoverInfo.gameObject.SetActive(false);
        trainCarHoverInfo.NullifyInfoData();
    }
    #endregion

    #region TrainTraits
    public void AddTrainTraits(SOTrainTrait soTrainTrait)
    {
        SOTrainTrait trait = soTrainTrait.Clone();
        soTrainTraits.Add(trait);

        for(int index = 0; index < trainCars.Count; index++)
        {
            InitTrainTraits(trait, trainCars[index]);
            GiveRewardTrainTraits(trait);
        }
    }

    public void RemoveTrainTraits(SOTrainTrait soTrainTrait)
    {
        for(int index = 0; index < trainCars.Count; index++)
        {
            InitTrainTraits(soTrainTrait, trainCars[index]);
            CancelRewardTrainTraits(soTrainTrait);
        }

        soTrainTraits.Remove(soTrainTrait);
    }

    public void InitTrainTraits(TrainCar trainCar)
    {
        for (int index = 0; index < soTrainTraits.Count; index++)
        {
            List<IReward> rewards = soTrainTraits[index].rewards;
            for (int rewardIndex = 0; rewardIndex < rewards.Count; rewardIndex++)
            {
                rewards[rewardIndex].InitReward(trainCar);
            }
        }
    }

    public void InitTrainTraits(SOTrainTrait soTrainTrait, TrainCar trainCar)
    {
        List<IReward> rewards = soTrainTrait.rewards;
        for (int rewardIndex = 0; rewardIndex < rewards.Count; rewardIndex++)
        {
            rewards[rewardIndex].InitReward(trainCar);
        }
    }

    public void GiveRewardTrainTraits()
    {
        for (int index = 0; index < soTrainTraits.Count; index++)
        {
            List<IReward> rewards = soTrainTraits[index].rewards;
            for (int rewardIndex = 0; rewardIndex < rewards.Count; rewardIndex++)
            {
                rewards[rewardIndex].giveReward();
            }
        }
    }

    public void GiveRewardTrainTraits(SOTrainTrait soTrainTrait)
    {
        List<IReward> rewards = soTrainTrait.rewards;
        for (int rewardIndex = 0; rewardIndex < rewards.Count; rewardIndex++)
        {
            rewards[rewardIndex].giveReward();
        }
    }
	#endregion

	#region Level
    public void AddExp(float exp)
    {
        if(level > requireExpList.Count - 2)
        {
            return;
        }

        curExp += exp;
        if (curExp >= requireExpList[level])
        {
            curExp -= requireExpList[level];
            LevelUp();
        }
        ExploreManager.Instance.guiLevelPanel.PrintExp();
        DataManager.Instance.SaveExp(curExp);
    }
	[Button]
	public void AddExp()
	{
		AddExp(1f);
    }

	public void LevelUp()
    {
        level++;
		maxCapacityTrainCars++;
        ResourceManager.Instance.GetPopulation().AddValue(2);
		SpareSpaceIndicator.Instance.UpdateSpareSpace(curCapacityTrainCars - 1, maxCapacityTrainCars - 1);
		ExploreManager.Instance.guiLevelPanel.PrintExp();
        DataManager.Instance.SaveLevel(level);
	}

    public void LevelSet(int _level)
    {
        level = _level;
        maxCapacityTrainCars = (SaveManager.Instance.trainDriver == ETrainDriver.Choi) ? level + 4 : level + 2;
        ResourceManager.Instance.GetPopulation().AddValue(2 * level);
        ExploreManager.Instance.guiLevelPanel.PrintExp();
    }

    public void AddEngineExp(float exp)
    {
        if (engineLevel > requireEngineExpList.Count - 1)
        {	
            return;
        }

        curEngineExp += exp;
        if (curEngineExp >= requireEngineExpList[engineLevel])
        {
            curEngineExp -= requireEngineExpList[engineLevel];
            if (engineLevel < requireEngineExpList.Count - 1) EngineLevelUp();
            else EngineGainHappiness(engineExpExceedHappiness);
        }
		HiddenStatManager.Instance?.Improvement?.UpdateGaugeAmount((int)curEngineExp, (int)requireEngineExpList[engineLevel], engineLevel);
        DataManager.Instance.SaveEngineExp(curEngineExp);
    }

    public void EngineLevelUp()
    {
        trainSpeed = defaultTrainSpeed * speedRateByEngineLevelList[engineLevel];
        ExploreManager.Instance.exploreMapTrainMovement.AddSpeed(trainSpeed);
        engineLevel++;
        DataManager.Instance.SaveEngineLevel(engineLevel);
		HiddenStatManager.Instance?.Improvement.UpdateRewardText(speedRateByEngineLevelList[engineLevel]);
        if (engineLevel == AchievementData.ACHIEVEMENT_INT_ENGINEGOD) SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_12);
    }

    public void EngineGainHappiness(int gainedHappiness)
    {
        trainCars[0].carContentBase.GainHappiness(gainedHappiness, trainCars[0].transform.position, Vector3.zero);
    }

    public void EngineLevelSet(int _level)
    {
        engineLevel = _level;
        DataManager.Instance.SaveEngineLevel(_level);
        HiddenStatManager.Instance?.Improvement.UpdateRewardText(speedRateByEngineLevelList[engineLevel]);
    }

    public void AddTrainSpeed(float _speed)
    {
        ExploreManager.Instance.exploreMapTrainMovement.AddSpeed(_speed);
    }
    #endregion

    #region Highlighting
    public void HighlightSynergyTrainCar(ECarTag carTag, bool isOn = true)
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].soTrainCar.CompareCarTags(carTag))
            {
                trainCars[index].HighlightTrainCar(isOn);
            }
        }
    }

    public void HighlightSameTrainCar(SOTrainCar soTrainCar, bool isOn = true)
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].soTrainCar.title.Equals(soTrainCar.title))
            {
                trainCars[index].HighlightTrainCar(isOn);
            }
        }
    }

    public void HighlightUpgradableTrainCar(SOTrainCar soTrainCar, bool isOn = true)
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index].soTrainCar.nextSOTrainCar != null &&
                trainCars[index].soTrainCar.title.Equals(soTrainCar.title) &&
                trainCars[index].soTrainCar.level == soTrainCar.level)
            {
                trainCars[index].HighlightTrainCar(isOn);
            }
        }
    }

    public void HighlightUpgradableTrainCar(TrainCar trainCar, bool isOn = true)
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            if (trainCars[index] != trainCar &&
                trainCars[index].soTrainCar.nextSOTrainCar != null &&
                trainCars[index].soTrainCar.title.Equals(trainCar.soTrainCar.title) &&
                trainCars[index].soTrainCar.level == trainCar.soTrainCar.level)
            {
                trainCars[index].HighlightTrainCar(isOn);
            }
        }
    }

    public void HighlightUpgradableTrainCarAuto()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            GameManager.Instance.player.train.HighlightUpgradableTrainCar(trainCars[index]);
            GameManager.Instance.player.inventory.HighlightUpgradableCard(trainCars[index].soTrainCar);
        }
    }

    public void OffAllHighlightTrainCar()
    {
        for (int index = 0; index < trainCars.Count; index++)
        {
            trainCars[index].HighlightTrainCar(false);
        }
    }
    #endregion
    #endregion

    #region PrivateMethod
    private void Start()
    {
        isBeforeInit = true;

        //driver init
        if (SaveManager.Instance.trainDriver == ETrainDriver.Choi)
        {
            selectCardOptionCount = 2;
        }
        else if (SaveManager.Instance.trainDriver == ETrainDriver.Cho)
        {
            selectCardOptionCount = 4;
        }


        GameManager.Instance.player.train = this;
        cameraController = Camera.main.GetComponent<CameraController>();
        InitTasks();

        defaultTrainSpeed = 0.3f;
        trainSpeed = defaultTrainSpeed;
        tierChanceByLevel = originalTierChanceByLevel.ToList();
		ExploreManager.Instance.guiExploreMap.NewNodeAction += AddExp;

        carTagCountList = Enumerable.Repeat(0, (int)(ECarTag.Max)).ToList();
        StartInitTrainSynergies();
        InitTrainSynergies();


        HiddenStatManager.Instance.StatInit();
        ExploreManager.Instance.guiLevelPanel.Init();
        TrashCan.Instance.Initialize();
        DataManager.Instance.LoadData();
        DataManager.Instance.LoadTrainData(this);

        StartInitTrainTraits();
        StartLaborTime(laborCycleTime);
        if (trainCarHoverInfo == null) trainCarHoverInfo = GameObject.Find("Canvas").transform.Find("TrainCarHoverInfo").GetComponent<GUITrainCarHoverInfo>();
        trainCarHoverInfo.InitTrainCarHoverInfo();

        isBeforeInit = false;
    }

    public void InitTrainCars()
    {
        trainCars = new List<TrainCar>();

        for (int index = 0; index < initSOTrainCars.Count; index++)
        {
            AddTrainCar(initSOTrainCars[index]);
        }
        if(initSOTrainCars.Count > 0)
        {
            if (trainCars.Count > 1) RepositionTrainCar(trainCars[1]);
        }
    }

    private void StartInitTrainTraits()
    {
        for (int index = 0; index < initTrainTraits.Count; index++)
        {
            AddTrainTraits(initTrainTraits[index]);
        }
    }

    private void StartInitTrainSynergies()
    {
        for (int index = 0; index < initTrainSynergies.Count; index++)
        {
            soTrainSynergies.Add(initTrainSynergies[index].Clone());
        }
    }

    private void InitTasks()
    {
        SOTask[] tasks = Resources.LoadAll<SOTask>("ScriptableObjects/Task");
        taskList.AddRange(tasks);
    }

    private void InitTrainSynergies()
    {
        for (int index = 0; index < soTrainSynergies.Count; index++)
        {
            for (int rewardsIndex = 0; rewardsIndex < soTrainSynergies[index].rewards.Count; rewardsIndex++)
            {
                soTrainSynergies[index].rewards[rewardsIndex].InitReward(this);

                switch(soTrainSynergies[index].CarTag)
                {
                    case ECarTag.RedFrostTemplar:
                        AddOrDeleteTrainCarAction += ((RewardWithCondition)(soTrainSynergies[index].rewards[rewardsIndex])).rewards[0].afterCycle;
                        break;
                    case ECarTag.Academy:
                        AddOrDeleteTrainCarAction += ((RewardWithCondition)(soTrainSynergies[index].rewards[rewardsIndex])).rewards[0].afterCycle;
                        break;
                    case ECarTag.GuildOfExplorers:
                        LaborSynergyAction += ((RewardWithCondition)(soTrainSynergies[index].rewards[rewardsIndex])).rewards[0].afterCycle;
                        break;
                }
            }
        }
    }

    private void CancelRewardTrainTraits(SOTrainTrait soTrainTrait)
    {
        List<IReward> rewards = soTrainTrait.rewards;
        for (int rewardIndex = 0; rewardIndex < rewards.Count; rewardIndex++)
        {
            rewards[rewardIndex].cancelReward?.Invoke();
        }
    }

    private void CountCarTags(TrainCar trainCar, bool isDelete = false)
    {
        if (trainCar.soTrainCar.isSecurityTrainCar)
        {
            return;
        }

        if(isDelete == false)
        {
            if (carTypeDic.ContainsKey(trainCar.soTrainCar.title))
            {
                carTypeDic[trainCar.soTrainCar.title].Add(trainCar);
            }
            else
            {
                for (int index = 0; index < trainCar.soTrainCar.carTags.Count; index++)
                {
                    carTagCountList[(int)(trainCar.soTrainCar.carTags[index])]++;
                }

                carTypeDic.Add(trainCar.soTrainCar.title, new List<TrainCar>());
                carTypeDic[trainCar.soTrainCar.title].Add(trainCar);
            }
        }
        else
        {
            carTypeDic[trainCar.soTrainCar.title].Remove(trainCar);

            if (carTypeDic[trainCar.soTrainCar.title].Count == 0)
            {
                for (int index = 0; index < trainCar.soTrainCar.carTags.Count; index++)
                {
                    carTagCountList[(int)(trainCar.soTrainCar.carTags[index])]--;
                }

                carTypeDic.Remove(trainCar.soTrainCar.title);
            }
        }
    }

    public object GetConditionTarget(int index = 0)
    {
        return null;
    }
    #endregion
}

[Serializable]
public struct guildOfExplorerSynergyData
{
    public int needRecordedArea;
    public int gainedHappiness;
}

[Serializable]
public struct tierChancesStruct
{
    public List<float> tierChances;
}

public enum ETrainDriver
{
    Lim,
    Choi,
    Cho,
    Max
}