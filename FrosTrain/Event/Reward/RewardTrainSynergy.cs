using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTrainSynergy : IReward
{
    #region PublicVariables
    public ECarTag carTag;
    [HideIf("@carTag == ECarTag.Culture || carTag == ECarTag.Entertainment || carTag == ECarTag.Media" +
        " || carTag == ECarTag.Large || carTag == ECarTag.Communication || carTag == ECarTag.CentralSupplyBureau" +
        " || carTag == ECarTag.FirstClass || carTag == ECarTag.TrainPower")]
    public int level;

    [ShowIf("carTag", ECarTag.Security)]
    public SOTrainCar soSecurityTrainCar;
    [ShowIf("carTag", ECarTag.Academy)]
    public SOTrainCar soAcademyTrainCar;
    [ShowIf("@carTag == ECarTag.Culture || carTag == ECarTag.Media || carTag == ECarTag.CentralSupplyBureau" +
		" || carTag == ECarTag.FirstClass || carTag == ECarTag.Communication || (carTag == ECarTag.FuelAndHammerBrotherhood && level != 0)")]
    public int weight;
    [ShowIf("@carTag == ECarTag.Entertainment")]
    public float chance;
    [ShowIf("@carTag == ECarTag.Entertainment || carTag == ECarTag.Large || (carTag == ECarTag.FuelAndHammerBrotherhood && level == 0)" +
        " || carTag == ECarTag.TrainPower || (carTag == ECarTag.RedFrostTemplar && level != 0)")]
    public float multiple;
    [ShowIf("@(carTag == ECarTag.RedFrostTemplar)")]
    public float maintain;
    [ShowIf("@(carTag == ECarTag.Academy)")]
    public int cycleCount;
    [ShowIf("@(carTag == ECarTag.Academy) && ((level == 1) || (level == 2))")]
    [SerializeField] public tierChancesStruct originalTierChancesAcademy;
    private tierChancesStruct tierChancesAcademy;

    public EActivateTiming _activateTiming { get; set; }
    public float _value { get; set; }
    public IRewardTarget rewardTarget { get; set; }
    public Action giveReward { get; set; }
    public Action cancelReward { get; set; }
    public Action afterCycle { get; set; }
    #endregion

    #region PrivateVariables
    private bool isDispose;
    private Train train;

    private List<TrainCar> trainCarList = new List<TrainCar>();
    private RewardHappinessForTrainCar firstClassSynergyReward;

    // CommunicationSynergy Value
    private int longestDistance = 0;
    private int longestIndex1 = 0;
    private int longestIndex2 = 0;
    #endregion

    #region PublicMethod
    public void InitReward(IRewardTarget _rewardTarget = null)
    {
        InitRewardTarget();
        InitRewardDelegate();
        isDispose = false;
        train = GameManager.Instance.player.train;
    }
    #endregion

    #region PrivateMethod
    private void InitRewardDelegate()
    {
        switch(carTag)
        {
            case ECarTag.Education:
                giveReward = GiveRewardEducationSynergy;
                break;
            case ECarTag.Security:
                giveReward = GiveRewardSecuritySynergy;
                break;
            case ECarTag.Culture:
                giveReward = GiveRewardCultureSynergy;
                break;
            case ECarTag.Entertainment:
                giveReward = GiveRewardEntertainmentSynergy;
                break;
            case ECarTag.TrainPower:
                giveReward = GiveRewardTrainPowerSynergy;
                break;
            case ECarTag.Media:
                giveReward = GiveRewardMediaSynergy;
                break;
            case ECarTag.Large:
                giveReward = GiveRewardLargeSynergy;
                break;
            case ECarTag.Communication:
                giveReward = GiveRewardCommunicationSynergy;
                break;
            case ECarTag.CentralSupplyBureau:
                giveReward = GiveRewardCentralSupplyBureauSynergy;
                break;
            case ECarTag.FirstClass:
                giveReward = GiveRewardFirstClassSynergy;
                break;
            case ECarTag.FuelAndHammerBrotherhood:
                giveReward = GiveRewardFuelAndHammerBrotherhoodSynergy;
                break;
            case ECarTag.RedFrostTemplar:
                giveReward = GiveRewardRedFrostTemplarSynergy;
                afterCycle = AddOrDeleteRedFrostTemplarSynergy;
                break;
            case ECarTag.ChildrenOfEngine:
                giveReward = GiveRewardChildrenOfEngineSynergy;
                break;
            case ECarTag.Academy:
                tierChancesAcademy = originalTierChancesAcademy;
                giveReward = GiveRewardAcademySynergy;
                afterCycle = AddOrDeleteAcademySynergy;
                break;
            case ECarTag.GuildOfExplorers:
                giveReward = GiveRewardGuildOfExplorersSynergy;
                afterCycle = AfterCycleGuildOfExplorersSynergy;
                break;
        }
    }

    private void InitRewardTarget(IRewardTarget _rewardTarget = null)
    {
        rewardTarget = _rewardTarget;
    }

    public void Dispose()
    {
        isDispose = true;
        //cancelReward?.Invoke();
        GC.SuppressFinalize(this);
    }

    ~RewardTrainSynergy()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    private void GiveRewardEducationSynergy()
    {
        trainCarList = train.GetCarList();
        for (int index = 0; index < trainCarList.Count; index++)
        {
            if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.Education))
            {
                switch (level)
                {
                    case 0:
                        if (trainCarList[index].carContentBase.isLaborerFull)
                        {
                            trainCarList[index].carContentBase.GiveEducationRewardsAction?.Invoke();
                        }
                        break;
                    case 1:
                        if (trainCarList[index].carContentBase.isLaborerFull)
                        {
                            trainCarList[index].carContentBase.GiveEducationRewardsAction?.Invoke();
                            trainCarList[index].carContentBase.GiveEducationRewardsAction?.Invoke();
                        }
                        break;
                    case 2:
                        if (trainCarList[index].carContentBase.laborer.Value > 0)
                        {
                            trainCarList[index].carContentBase.GiveEducationRewardsAction?.Invoke();
                            trainCarList[index].carContentBase.GiveEducationRewardsAction?.Invoke();
                        }
                        break;
                }
            }
        }
    }

    private void GiveRewardSecuritySynergy()
    {
        if (train.isBeforeInit) return;

        switch (level)
        {
            case 0:
                if(train.securityTrainCar != null)
                {
                    int carIndex = train.securityTrainCar.carIndex;
                    train.DeleteTrainCar(train.securityTrainCar);
                    train.RepositionTrainCar(train.GetCarList()[1]);
                }
                break;
            case 1:
            case 2:
                if(train.securityTrainCar == null)
                {
                    train.RepositionTrainCar(train.GetCarList()[train.GetCarLastIndex()]);
                    train.AddTrainCar(soSecurityTrainCar);
                    if(train.securityTrainCar == null)
                    {
                        Debug.Log("Capacity Over!!!");
                    }
                }
                else
                {
                    if(train.securityTrainCar.soTrainCar.title == soSecurityTrainCar.title)
                    {
                        return;
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.EventPopup);
                        AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainDropMerge);
                        EffectManager.Instance.PlayEffect(EffectManager.ECommonEffectType.trainCarUpgrade, train.securityTrainCar.transform.position);
                        train.securityTrainCar.carContentBase.CycleCarContentBase();
                        train.securityTrainCar.ChangeCar(soSecurityTrainCar);
                    }
                }
                break;
        }
    }

    private void GiveRewardCultureSynergy()
    {
        trainCarList = new List<TrainCar>();
        List<TrainCar> carList = train.GetCarList();
        for (int index = 0; index < carList.Count; index++)
        {
            if (carList[index].soTrainCar.CompareCarTags(ECarTag.Culture) &&
                carList[index].carContentBase.laborer.Value > 0)
            {
                trainCarList.Add(carList[index]);
            }
        }

        if (trainCarList.Count > 0)
        {
            train.laboredTrainCarCount++;
            train.LaborAction += LaborActionCultureSynergy;
        }
    }

    private void LaborActionCultureSynergy()
    {
		train.StartCoroutine(train.DelayedGainHappiness(trainCarList[0], trainCarList[0].carContentBase.laborer.Value * weight, trainCarList[0].transform.position, Vector3.zero));
        for (int index = 1; index < trainCarList.Count; index++)
        {
			train.StartCoroutine(train.ImmediatelyGainHappiness(trainCarList[index], trainCarList[index].carContentBase.laborer.Value * weight, trainCarList[index].transform.position, Vector3.zero));
        }

        trainCarList.Clear();
        train.LaborAction -= LaborActionCultureSynergy;
    }

    private void GiveRewardEntertainmentSynergy()
    {
        trainCarList = train.GetCarList();
        for (int index = 0; index < trainCarList.Count; index++)
        {
            if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.Entertainment) &&
                trainCarList[index].carContentBase.laborer.Value > 0 && 
                UnityEngine.Random.value < chance)
            {
                trainCarList[index].carContentBase.ModifyHappinessListWithCycleByRate(multiple - 1);
            }
        }
    }

    private void GiveRewardTrainPowerSynergy()
    {
        if (multiple == 0) return;

        trainCarList = train.GetCarList();
        for (int index = 0; index < trainCarList.Count; index++)
        {
            if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.TrainPower))
            {
                trainCarList[index].carContentBase.trainPowerSynergyMultiple = multiple;
                for (int count = 0; count < trainCarList[index].carContentBase.laborer.Value; count++)
                {
                    trainCarList[index].carContentBase.GiveTrainPowerRewardsAction?.Invoke();
                }
            }
        }
    }

    private void GiveRewardMediaSynergy()
    {
        trainCarList = train.GetCarList();
        for (int index = 1; index < trainCarList.Count; index++)
        {
            trainCarList[index].carContentBase.GainHappiness(weight, trainCarList[index].transform.position, Vector3.zero);
        }
    }

    private void GiveRewardLargeSynergy()
    {
        trainCarList = train.GetCarList();
        for (int index = 0; index < trainCarList.Count; index++)
        {
            if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.Large))
            {
                trainCarList[index].carContentBase.ModifyHappinessListWithCycleByRate(multiple - 1);
            }
        }
    }

    private void GiveRewardCommunicationSynergy()
    {
        trainCarList = train.GetCarList();
        longestDistance = 0;
        longestIndex1 = 0;
        longestIndex2 = 0;
        for (int index1 = 0; index1 < trainCarList.Count; index1++)
        {
            if (trainCarList[index1].soTrainCar.CompareCarTags(ECarTag.Communication))
            {
                for (int index2 = trainCarList.Count - 1; index2 > 0; index2--)
                {
                    if (trainCarList[index2].soTrainCar.CompareCarTags(ECarTag.Communication) &&
                        trainCarList[index1].soTrainCar.title.Equals(trainCarList[index2].soTrainCar.title) != true)
                    {
                        int distance = Math.Abs(index1 - index2);
                        if (distance > longestDistance)
                        {
                            longestIndex1 = index1;
                            longestIndex2 = index2;
                            longestDistance = distance;
                        }
                        break;
                    }
                }
            }
        }

        if(longestDistance <= 0)
        {
            return;
        }

        train.laboredTrainCarCount++;
        train.LaborAction += LaborActionCommunicationSynergy;
    }

    private void LaborActionCommunicationSynergy()
    {
        int index1 = Math.Min(longestIndex1, longestIndex2);
        int index2 = Math.Max(longestIndex1, longestIndex2);
		train.StartCoroutine(train.DelayedGainHappiness(trainCarList[index1], weight, trainCarList[index1].transform.position, Vector3.zero));
        for (int index = index1 + 1; index <= index2; index++)
        {
			train.StartCoroutine(train.ImmediatelyGainHappiness(trainCarList[index], weight, trainCarList[index].transform.position, Vector3.zero));
        }

        train.LaborAction -= LaborActionCommunicationSynergy;
    }

    private void GiveRewardCentralSupplyBureauSynergy()
    {
        train.laborCycleTimeWeightSynergy = weight;
    }

    private void GiveRewardFirstClassSynergy()
    {

    }

    private void GiveRewardFuelAndHammerBrotherhoodSynergy()
    {
        trainCarList = train.GetCarList();

        switch (level)
        {
            case 0:
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.FuelAndHammerBrotherhood) &&
                        trainCarList[index].carContentBase.laborer.Value > 0)
                    {
                        trainCarList[index].carContentBase.AddHappinessWithCycle(-Mathf.FloorToInt((trainCarList[index].carIndex - 1) * multiple));
                    }
                }
                break;
            case 1:
            case 2:
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.FuelAndHammerBrotherhood) &&
                        trainCarList[index].carContentBase.laborer.Value > 0)
                    {
                        trainCarList[index].carContentBase.laborCount += weight;
                    }
                }
                break;
        }
    }

    private void GiveRewardRedFrostTemplarSynergy()
    {
        switch (level)
        {
            case 0:
                break;
            case 1:
            case 2:
                trainCarList = train.GetCarList();
                GameManager.Instance.player.train.trainingMultipleBySynergy = multiple;
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.RedFrostTemplar))
                    {
                        for (int count = 0; count < trainCarList[index].carContentBase.laborer.Value; count++)
                        {
                            trainCarList[index].carContentBase.GiveRedFrostTemplarRewardsAction?.Invoke();
                        }
                    }
                }

                // 심사 카운트
                train.trainingEvaluationCount--;
                if (train.trainingEvaluationCount <= 0)
                {
                    TrainingEvaluation();
                    train.trainingEvaluationCount = train.needTrainingEvaluation;
                    HiddenStatManager.Instance.Training.UpdateTurnEvaluation();
                }
                else
                {
                    HiddenStatManager.Instance.Training.UpdateTurn(train.trainingEvaluationCount);
                    DataManager.Instance.SaveTrainingEvaluationCountData(train.trainingEvaluationCount);
                }
                break;
            case 3:
				trainCarList = train.GetCarList();
                GameManager.Instance.player.train.trainingMultipleBySynergy = multiple;
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.RedFrostTemplar))
                    {
                        for (int count = 0; count < trainCarList[index].carContentBase.laborer.Value; count++)
                        {
                            trainCarList[index].carContentBase.GiveRedFrostTemplarRewardsAction?.Invoke();
                        }
                    }
                }

				// 심사 카운트
				TrainingEvaluation();
				train.trainingEvaluationCount = 1;
                HiddenStatManager.Instance.Training.UpdateTurnEvaluation();
                break;
        }
    }

    private void TrainingEvaluation()
    {
        trainCarList = new List<TrainCar>();
        List<TrainCar> carList = train.GetCarList();
        for (int index = 0; index < carList.Count; index++)
        {
            if (carList[index].soTrainCar.CompareCarTags(ECarTag.RedFrostTemplar))
            {
                trainCarList.Add(carList[index]);
            }
        }

        if (trainCarList.Count > 0)
        {
            train.laboredTrainCarCount++;
            train.LaborAction += LaborActionRedFrostTemplarSynergy;
        }
    }

    private void LaborActionRedFrostTemplarSynergy()
    {
        train.StartCoroutine(train.DelayedGainHappinessForTraining(trainCarList[0], Mathf.FloorToInt(trainCarList[0].soTrainCar.training), trainCarList[0].transform.position, Vector3.zero, maintain));
        trainCarList[0].soTrainCar.training *= maintain;
        for (int index = 1; index < trainCarList.Count; index++)
        {
			train.StartCoroutine(train.ImmediatelyGainHappiness(trainCarList[index], Mathf.FloorToInt(trainCarList[index].soTrainCar.training), trainCarList[index].transform.position, Vector3.zero));
            trainCarList[index].soTrainCar.training *= maintain;
        }

        trainCarList.Clear();
        train.LaborAction -= LaborActionRedFrostTemplarSynergy;
    }

    private void AddOrDeleteRedFrostTemplarSynergy()
    {
        int carTagCount = train.carTagCountList[(int)ECarTag.RedFrostTemplar];

        switch (level)
        {
            case 0:
                if (carTagCount < 3)
                {
                    if (train.previousRedFrostTemplarLevel == level + 1)
                    {
                        train.trainingEvaluationCount = train.needTrainingEvaluation;
                        HiddenStatManager.Instance.Training.UpdateTurn(train.trainingEvaluationCount);
                        DataManager.Instance.SaveTrainingEvaluationCountData(train.trainingEvaluationCount);
                    }

                    train.previousRedFrostTemplarLevel = level;
                }
                break;
            case 1:
                break;
            case 2:
                if (carTagCount == 4)
                {
                    if (train.previousRedFrostTemplarLevel == level + 1)
                    {
                        train.trainingEvaluationCount = train.needTrainingEvaluation;
                        HiddenStatManager.Instance.Training.UpdateTurn(train.trainingEvaluationCount);
                        DataManager.Instance.SaveTrainingEvaluationCountData(train.trainingEvaluationCount);
                    }

                    train.previousRedFrostTemplarLevel = level;
                }
                break;
            case 3:
                if (carTagCount >= 5)
                {
                    if (train.trainingEvaluationCount > 1)
                    {
                        train.trainingEvaluationCount = 1;
                        HiddenStatManager.Instance.Training.UpdateTurn(train.trainingEvaluationCount);
                        DataManager.Instance.SaveTrainingEvaluationCountData(train.trainingEvaluationCount);
                    }

                    train.previousRedFrostTemplarLevel = level;
                }
                break;
        }
    }

    private void GiveRewardChildrenOfEngineSynergy()
    {
        switch(level)
        {
            case 0:
                break;
            case 1:
                trainCarList = train.GetCarList();
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.ChildrenOfEngine) 
                        && trainCarList[index].carContentBase.laborer.Value == 1)
                    {
                        trainCarList[index].carContentBase.AddZealWithCycle(1);
                    }
                }
                break;
            case 2:
                trainCarList = train.GetCarList();
                for (int index = 0; index < trainCarList.Count; index++)
                {
                    if (trainCarList[index].soTrainCar.CompareCarTags(ECarTag.ChildrenOfEngine))
                    {
                        trainCarList[index].carContentBase.AddZealWithCycle(1);
                    }
                }
                break;
        }
    }

    private void GiveRewardAcademySynergy()
    {
        switch(level)
        {
            case 0:
                break;
            case 1:
            case 2:
                train.academyCycle--;
                if (train.academyCycle <= 0)
                {
                    train.academyCycle = cycleCount;

                    List<int> tiers = new List<int>();
                    List<SORandomRewards.RewardOptionTypes> optionTypes = new List<SORandomRewards.RewardOptionTypes>();
                    optionTypes.Add(SORandomRewards.RewardOptionTypes.TrainCar);

                    //------------------------ Tier Select ------------------------
                    float maxChance = 0f;
                    List<float> tierChances = tierChancesAcademy.tierChances;
                    for (int tier = 0; tier < tierChances.Count; tier++)
                    {
                        maxChance += tierChances[tier];
                    }

                    float randomChance = UnityEngine.Random.Range(0f, maxChance);
                    float curChance = 0f;
                    for (int tier = 0; tier < tierChances.Count; tier++)
                    {
                        if (randomChance >= curChance && randomChance < curChance + tierChances[tier])
                        {
                            tiers.Add(tier + 1);
                            break;
                        }

                        curChance += tierChances[tier];
                    }

					DataManager.Instance.SaveAcademyCycleData(train.academyCycle);
					HiddenStatManager.Instance.Academy.UpdateAmount(train.academyCycle);

					if (tiers.Count <= 0)
                    {
                        if (level >= 2) GameManager.Instance.player.inventory.AddCard(soAcademyTrainCar, true);
                        return;
                    }
                    //-------------------------------------------------------------
                    //--------------------- RandomList Select ---------------------
                    List<SORandomRewards> selectedRandomList = new List<SORandomRewards>();
                    foreach (SORandomRewards soRandomRewards in GameEventManager.Instance.soRandomRewardsList)
                    {
                        if (soRandomRewards.CheckTiers(tiers) &&
                            soRandomRewards.CheckOptionTypes(optionTypes))
                        {
                            selectedRandomList.Add(soRandomRewards.Clone());
                        }
                    }
                    if (selectedRandomList.Count <= 0)
                    {
						return;
                    }
                    //-------------------------------------------------------------
                    //------------------------ Card Select ------------------------
                    RewardsAndChance selectedReward = default;
                    maxChance = 0f;
                    foreach (SORandomRewards soRandomReward in selectedRandomList)
                    {
                        foreach (RewardsAndChance reward in soRandomReward.rewards)
                        {
                            if (reward.rewards[0] is RewardTrainCar && ((RewardTrainCar)(reward.rewards[0])).soTrainCar.CompareCarTags(ECarTag.Academy) == false)
                            {
                                reward.chance = 0f;
                            }

                            maxChance += reward.chance;
                        }
                    }

                    if (maxChance <= 0f)
                    {
                        tierChancesAcademy.tierChances[tiers[0] - 1] = 0f;
                        GiveRewardAcademySynergy();
                        return;
                    }

                    randomChance = UnityEngine.Random.Range(0f, maxChance);
                    curChance = 0f;
                    foreach (SORandomRewards soRandomReward in selectedRandomList)
                    {
                        foreach (RewardsAndChance reward in soRandomReward.rewards)
                        {
                            if (randomChance >= curChance && randomChance < curChance + reward.chance)
                            {
                                selectedReward = reward;
                                break;
                            }

                            curChance += reward.chance;
                        }

                        if (selectedReward != default)
                        {
                            InventoryCard card = GameManager.Instance.player.inventory.AddCard(((RewardTrainCar)(selectedReward.rewards[0])).soTrainCar, true);
                            if (card != null) GameEventManager.Instance.ReduceChanceSOTrainCar(((RewardTrainCar)(selectedReward.rewards[0])).soTrainCar);
                            break;
                        }
                    }
                    //-------------------------------------------------------------
                }
				DataManager.Instance.SaveAcademyCycleData(train.academyCycle);
				HiddenStatManager.Instance.Academy.UpdateAmount(train.academyCycle);
				break;
        }
    }

    private void AddOrDeleteAcademySynergy()
    {
        int carTagCount = train.carTagCountList[(int)ECarTag.Academy];

        switch (level)
        {
            case 0:
                if (carTagCount < 2)
                {
                    if (train.previousAcademyLevel == level + 1)
                    {
                        train.academyCycle = cycleCount;
                        DataManager.Instance.SaveAcademyCycleData(train.academyCycle);
                        HiddenStatManager.Instance.Academy.UpdateAmount(train.academyCycle);
                    }

                    train.previousAcademyLevel = level;
                }
                break;
            case 1:
                if(carTagCount >= 2 && carTagCount < 4)
                {
                    if (train.previousAcademyLevel == level + 1)
                    {
                        train.academyCycle = cycleCount;
                        DataManager.Instance.SaveAcademyCycleData(train.academyCycle);
                        HiddenStatManager.Instance.Academy.UpdateAmount(train.academyCycle);
                    }

                    train.previousAcademyLevel = level;
                }
                break;
            case 2:
                if (carTagCount >= 4)
                {
                    if (train.academyCycle > cycleCount)
                    {
                        train.academyCycle = cycleCount;
                        DataManager.Instance.SaveAcademyCycleData(train.academyCycle);
                        HiddenStatManager.Instance.Academy.UpdateAmount(train.academyCycle);
                    }

                    train.previousAcademyLevel = level;
                }
                break;
        }
    }

    private void GiveRewardGuildOfExplorersSynergy()
    {
        switch(level)
        {
            case 1:
                train.recordedArea++;
                DataManager.Instance.SaveRecordedAreaData(train.recordedArea);
				HiddenStatManager.Instance.Pioneering.UpdateAmount(train.recordedArea);
				int gainedHappiness = 0;
				for (int index = 0; index < train.guildOfExplorersSynergyTable.Count; index++)
				{
					if (train.guildOfExplorersSynergyTable[index].needRecordedArea > train.recordedArea)
					{
						break;
					}

					gainedHappiness = train.guildOfExplorersSynergyTable[index].gainedHappiness;
				}
				HiddenStatManager.Instance.Pioneering.UpdateReward(gainedHappiness);
                if (train.recordedArea >= AchievementData.ACHIEVEMENT_INT_THEEXPLORER) SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_17);
				break;
        }
    }

    private void AfterCycleGuildOfExplorersSynergy()
    {
        if (train.carTagCountList[(int)ECarTag.GuildOfExplorers] >= 3 && train.guildOfExplorersSynergyTable[0].needRecordedArea <= train.recordedArea)
        {
            train.laboredTrainCarCount++;
            train.LaborAction += LaborActionGuildOfExplorersSynergy;
        }
    }

    private void LaborActionGuildOfExplorersSynergy()
    {
        int gainedHappiness = 0;
        for (int index = 0; index < train.guildOfExplorersSynergyTable.Count; index++)
        {
            if (train.guildOfExplorersSynergyTable[index].needRecordedArea > train.recordedArea)
            {
                break;
            }

            gainedHappiness = train.guildOfExplorersSynergyTable[index].gainedHappiness;
        }
		train.StartCoroutine(train.DelayedGainHappiness(train.GetCarList()[0], gainedHappiness, train.GetCarList()[0].transform.position, Vector3.zero));
        train.LaborAction -= LaborActionGuildOfExplorersSynergy;
    }
    #endregion
}