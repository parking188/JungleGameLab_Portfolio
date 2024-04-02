using System;
using System.Collections;
using UnityEngine;


public class GameEvent : IDisposable
{
	#region PublicVariables
	public SOEventData eventSOData;
	#endregion

	#region PrivateVariables
	private bool isDispose;
	private bool isEventOn;
    #endregion

    #region PublicMethod
    public GameEvent(SOEventData _eventData)
    {
		isDispose = false;
        eventSOData = _eventData.Clone();
        InitGameEvent();
        
    }

    public void OccurGameEvent()
    {
        if(eventSOData.isReward)
        {
            GiveRewards();
        }
        else
        {
            GameEventManager.Instance.StartCoroutine(GameEventManager.Instance.PopupEvent(this));
        }

        GameEventManager.Instance.StartCoroutine(EventOnAfterDelay());
    }

    public void GiveRewards()
    {
        for(int index = 0; index < eventSOData.rewards.Count; index++)
        {
            eventSOData.rewards[index].giveReward();
        }
    }

    public void ForcedOccurGameEvent()
    {
        if (eventSOData.eventCount > 0)
        {
            isEventOn = false;
            eventSOData.eventCount--;
            OccurGameEvent();
        }

        if (eventSOData.eventCount <= 0)
        {
            DeleteConditions();
        }
    }

    ~GameEvent()
    {
        if (!isDispose)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        Debug.Log("EventDelete");
		isDispose = true;
		eventSOData.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion

    #region PrivateMethod
    

	private void InitGameEvent()
    {
        isEventOn = false;
        InitConditionList();
        InitRewardList();
        GameEventManager.Instance.StartCoroutine(EventOnAfterDelay());
    }

	private void InitConditionList()
	{
        for(int index = 0; index < eventSOData.conditions.Count; index++)
        {
            eventSOData.conditions[index].InitCondition();
            eventSOData.conditions[index].actionWhenConditionMet += CheckAndOccur;
        }
    }

    private void InitRewardList()
    {
        for(int index = 0; index < eventSOData.rewards.Count; index++)
        {
            eventSOData.rewards[index].InitReward();
        }
    }

    private void DeleteConditions()
    {
        if (eventSOData.conditions.Count > 0)
        {
            foreach (ICondition condition in eventSOData.conditions)
            {
				condition.Dispose();
            }
        }
    }

    private bool CheckConditions()
	{
		if(isEventOn == false)
		{
			return false;
		}

		if(eventSOData.conditions.Count <= 0)
		{
            return true;
        }

		foreach(ICondition condition in eventSOData.conditions)
		{
            if(condition.isConditionMet == false)
            {
                return false;
            }
		}

		return true;
	}

	private void CheckAndOccur()
	{
		if(CheckConditions())
		{
            if (eventSOData.isForced)
            {
                ForcedOccurGameEvent();
            }
            else
            {
                AlarmEvent();
            }
        }
	}

    private void AlarmEvent()
    {
        if (eventSOData.eventCount > 0)
        {
            GameEventManager.Instance.AlarmEventButton(this);
            isEventOn = false;
            eventSOData.eventCount--;
        }

        if (eventSOData.eventCount <= 0)
        {
            DeleteConditions();
        }
    }

    private IEnumerator EventOnAfterDelay()
	{
		yield return new WaitForSeconds(eventSOData.eventCooltime);
		isEventOn = true;
		CheckAndOccur();
    }
    #endregion
}
