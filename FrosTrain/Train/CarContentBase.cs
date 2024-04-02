using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarContentBase : MonoBehaviour
{
    #region PublicVariables
    public SOTrainCar soTrainCar;
    public bool isLaborerFull;
    public Laborer laborer;
    public List<EffectLaborerStack> effectLaborerStacks;
    public float laborerStackInterval = 0.8f;
    public int laborCount = 1;
    public int defaultLaborCount = 1;
    
    [HideInInspector] public Action<int> visitAction;
    [HideInInspector] public float trainPowerSynergyMultiple;

    public Action GiveEducationRewardsAction;
    public Action GiveTrainPowerRewardsAction;
    public Action GiveRedFrostTemplarRewardsAction;
    #endregion

    #region PrivateVariables
    public List<int> gainedHappinessList { get; private set; } = new List<int>();
    public float gainedTraining { get; private set; }
    public int gainedZeal { get; private set; }
    public float gainedEngineExp { get; private set; }

    public TrainCar trainCar { get; private set; }
    private float currentLaborerStackPosY = 0f;
    private float delayTime = 0.2f;
    private float textAddPosY = 0.5f;
    #endregion

    #region PublicMethod
    public void InitCarContentBase(SOTrainCar data)
    {
        soTrainCar = data;
        effectLaborerStacks = new List<EffectLaborerStack>();
        trainCar = GetComponent<TrainCar>();
        InitLaborer();
    }

    public virtual bool CheckTrainCarLabor()
    {
        if (laborer.Value > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool TryAddOneLaborer()
    {
        if(isLaborerFull)
        {
            return false;
        }

        laborer.AddValue(1);
        isLaborerFull = (laborer.Value >= laborer.MaxValue) ? true : false;
        GameManager.Instance.player.train.CountLaborerMove(this);
        return true;
    }

    public virtual IEnumerator CallEffectLaborerStack(float effectTime)
    {
        yield return new WaitForSeconds(effectTime);
        Vector3 effectPosition = transform.position;
        currentLaborerStackPosY += laborerStackInterval;
        effectPosition.y += currentLaborerStackPosY;
        EffectLaborerStack tempEffectLaborerStack = EffectManager.Instance.InstantiateLaborerStack(gameObject, currentLaborerStackPosY);
        tempEffectLaborerStack.StartCoroutine(DelayedDeleteEffectLaborerStack(tempEffectLaborerStack));
        effectLaborerStacks.Add(tempEffectLaborerStack);
		AudioManager.Instance.PlaySFXOneShot((AudioManager.ESoundType)UnityEngine.Random.Range((int)AudioManager.ESoundType.PeopleEnter1, (int)AudioManager.ESoundType.PeopleEnter3));
		visitAction?.Invoke(laborer.Value);
    }

    public virtual void OutAllLaborer()
    {
        laborer.AddValue(-laborer.MaxValue);
        isLaborerFull = (laborer.Value >= laborer.MaxValue) ? true : false;
        laborCount = defaultLaborCount;
    }

    public virtual void ClearEffectLaborerStacks()
    {
        currentLaborerStackPosY = 0f;
        int effectLaborerStacksCount = effectLaborerStacks.Count;
        for(int index = 0; index < effectLaborerStacksCount; index++)
        {
            effectLaborerStacks[index].Remove();
        }
        effectLaborerStacks.Clear();
    }

    public virtual void LaborTrainCar()
    {
        for(int count = 0; count < laborCount; count++)
        {
            for (int laborCount = 0; laborCount < laborer.Value; laborCount++)
            {
                //Debug.Log(gameObject.name + " : " + laborer.Value);
                for (int index = 0; index < soTrainCar.rewards.Count; index++)
                {
                    soTrainCar.rewards[index].giveReward?.Invoke();
                }
            }

            for (int index = 0; index < soTrainCar.rewards.Count; index++)
            {
                soTrainCar.rewards[index].afterCycle?.Invoke();
            }
        }
    }

    public virtual void SynergyTrainCar()
    {
        for(int count = 0; count < laborCount; count++)
        {
            for (int laborCount = 0; laborCount < laborer.Value; laborCount++)
            {
                for (int index = 0; index < soTrainCar.carSynergys.Count; index++)
                {
                    soTrainCar.carSynergys[index].giveReward?.Invoke();
                }
            }
        }
    }

    public virtual void SynergyAfterTrainCar()
    {
        for(int count = 0; count < laborCount; count++)
        {
            for (int laborCount = 0; laborCount < laborer.Value; laborCount++)
            {
                for (int index = 0; index < soTrainCar.carSynergys.Count; index++)
                {
                    soTrainCar.carSynergys[index].afterCycle?.Invoke();
                }
            }
        }
    }

    public virtual void SynergyCancelTrainCar()
    {
        for(int count = 0; count < laborCount; count++)
        {
            for (int laborCount = 0; laborCount < laborer.Value; laborCount++)
            {
                for (int index = 0; index < soTrainCar.carSynergys.Count; index++)
                {
                    soTrainCar.carSynergys[index].cancelReward?.Invoke();
                }
            }
        }
    }

    //public virtual void InvokeAdditionalRewards()
    //{
    //    for(int index = 0; index < soTrainCar.additionalRewards.Count; index++)
    //    {
    //        soTrainCar.additionalRewards[index].giveReward?.Invoke();
    //    }
    //}

    public virtual void AddHappinessWithCycle(int value)
    {
        gainedHappinessList.Add(value);
    }

    public virtual void ModifyHappinessListWithCycleByRate(float rate)
    {
        for (int index = 0; index < gainedHappinessList.Count; index++)
        {
            gainedHappinessList[index] += Mathf.RoundToInt(gainedHappinessList[index] * rate);
        }
    }

    public virtual void AddEngineExpWithCycle(float value)
    {
        gainedEngineExp += value;
    }

    public virtual void AddTrainingWithCycle(float value)
    {
        gainedTraining += value;
    }

    public virtual void AddZealWithCycle(int value)
    {
        gainedZeal += value;
    }

    public virtual void ResetAllResourcesWithCycle()
    {
        gainedHappinessList.Clear();
        gainedEngineExp = 0;
        gainedTraining = 0;
        gainedZeal = 0;
    }

    public virtual void RefreshLaborerStackPosition()
    {
        for(int index = 0; index < effectLaborerStacks.Count; index++)
        {
            Vector3 movePosition = effectLaborerStacks[index].transform.position;
            movePosition.x = transform.position.x;
            effectLaborerStacks[index].transform.position = movePosition;
        }
    }

    public virtual void CycleCarContentBase()
    {
		ClearEffectLaborerStacks();
        GainHappiness();
        trainCar.train.AddEngineExp(gainedEngineExp);
        trainCar.soTrainCar.training += gainedTraining;
        trainCar.train.curTotalTraining += gainedTraining;
        DataManager.Instance?.SaveCurTotalTrainingData(trainCar.train.curTotalTraining);
		HiddenStatManager.Instance?.Training.UpdateAmount((int)trainCar.train.curTotalTraining);
        ResourceManager.Instance?.GetZeal().AddValue(gainedZeal);
        DataManager.Instance?.SaveZealData(gainedZeal);
        ResetAllResourcesWithCycle();
        trainCar.train.CheckIsSynergyInRange(trainCar);

        if (ResourceManager.Instance?.GetZeal().Value == AchievementData.ACHIEVEMENT_INT_SHEEP) SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_10);
    }

    public virtual void GainHappiness()
    {
        float time = 0f;
        Vector3 textPos = Vector3.zero;
        for (int index = 0; index < gainedHappinessList.Count; index++)
        {
            int gainedHappiness = gainedHappinessList[index];
            trainCar.train.StartCoroutine(GainHappinessCoroutine(gainedHappiness, time, transform.position, textPos));
            time += delayTime;
            textPos.y += textAddPosY;
        }
    }

    public IEnumerator GainHappinessCoroutine(float _gainedHappiness, float _time, Vector3 _position, Vector3 _textPos)
    {
        yield return new WaitForSeconds(_time);
        GainHappiness(_gainedHappiness, _position, _textPos);
    }

    public virtual void GainHappiness(float _gainedHappiness, Vector3 _position, Vector3 _textPosFromPosition)
    {
        Vector3 textPos = _position + _textPosFromPosition;

        if (this != null)
        {
            _position = transform.position;
            textPos = transform.position + _textPosFromPosition;
        }

        if (_gainedHappiness > 0)
        {
            EffectManager.Instance?.PlayOneShot(EffectManager.EParticleEffectType.happinessBomb, _position, (int)_gainedHappiness);
        }
        else if (_gainedHappiness < 0)
        {
            EffectManager.Instance?.PlayOneShot(EffectManager.EParticleEffectType.UnhappinessBomb, _position, -(int)_gainedHappiness);
        }
		EffectManager.Instance?.InstantiateCounterText(textPos, (int)_gainedHappiness);
		ResourceManager.Instance?.GetHappiness().AddValue(_gainedHappiness);
	}
    #endregion

    #region PrivateMethod
    protected virtual void InitLaborer()
    {
        if (laborer == null) laborer = gameObject.AddComponent<Laborer>();
        laborer.AddMaxValue(soTrainCar.maxLaborer);
    }

    private void OnDestroy()
    {
        if (laborer != null) Destroy(laborer);
    }

    private IEnumerator DelayedDeleteEffectLaborerStack(EffectLaborerStack tempEffectLaborerStack)
    {
        yield return new WaitForSeconds(GameManager.Instance.player.train.laborCycleTime);
        effectLaborerStacks.Remove(tempEffectLaborerStack);
        tempEffectLaborerStack.Remove();
    }
    #endregion
}
