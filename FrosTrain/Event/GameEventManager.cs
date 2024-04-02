using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
	#region PublicVariables
    public GUIEventPopup eventPopupUI;
    public GUIRandomSelectPopup randomSelectPopupUI;
    public GameObject eventAlarmPanel;
    public GameObject eventStartButtonPrefab;
    public RectTransform eventButtonPos;
    public GameObject rewardSelectButtonPrefab;
	public List<GameEvent> gameEvents = new List<GameEvent>();
	public List<SOEventData> initEvents = new List<SOEventData>();
    public List<SORandomRewards> soRandomRewardsList = new List<SORandomRewards>();
    public bool isTutorial;
    [Header("CurrentAgendaEvent")]
    public List<GUIEventStartButton> eventStartBtns = new();
    public Dictionary<SOEventData, float> curAgendaEvents = new Dictionary<SOEventData,float>();
    public ScenarioPanel scenarioPanel = new ScenarioPanel();

    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Start()
    {
        if (eventPopupUI == null) eventPopupUI = GameObject.Find("Canvas").transform.Find("Event Popup").GetComponent<GUIEventPopup>();
        if (randomSelectPopupUI == null) randomSelectPopupUI = GameObject.Find("Canvas").transform.Find("RewardSelectPanel").GetComponent<GUIRandomSelectPopup>();
        if (eventAlarmPanel == null) eventAlarmPanel = GameObject.Find("UICanvas").transform.Find("AgendaPanel/Viewport/Content").gameObject;
        if (GameManager.Instance.timeSystem.isNew && !isTutorial)
        {
            //SaveManager.Instance.AddSteamUserStat("playCount", 1);
            if (scenarioPanel != null)
                scenarioPanel.Init(InitGameEvent);
            else 
                InitGameEvent();
        }
        InitEvent();
    }

    private void InitEvent()
    {

        InitRandomRewards();
        DataManager.Instance.LoadRandomRewardValue();
        DataManager.Instance.LoadagendaEventList();
    }

    public bool CheckActiveEvent()
    {
        return eventPopupUI.transform.GetChild(0).gameObject.activeSelf || randomSelectPopupUI.gameObject.activeSelf;
    }
    public void RegisterGameEvent(SOEventData eventData)
	{
		gameEvents.Add(new GameEvent(eventData));
	}

    public void DeleteGameEvent(GameEvent gameEvent)
    {
        gameEvents.Remove(gameEvent);
        gameEvent.Dispose();
    }

    public void AlarmEventButton(GameEvent gameEvent)
    {
        GameObject button;

        button = Instantiate(eventStartButtonPrefab, eventButtonPos.position, Quaternion.identity, eventAlarmPanel.transform);

        GUIEventStartButton eventStartButton = button.GetComponent<GUIEventStartButton>();
        eventStartButton.InitEventStartButton(gameEvent);
        eventStartBtns.Add(eventStartButton);
    }

    public void AlarmEventButtonInit(SOEventData _soEvent, float _curTime)
    {
        GameObject button;


        button = Instantiate(eventStartButtonPrefab, eventButtonPos.position, Quaternion.identity, eventAlarmPanel.transform);

        GUIEventStartButton eventStartButton = button.GetComponent<GUIEventStartButton>();
        eventStartButton.InitEventStartButton(new GameEvent(_soEvent));
        eventStartButton.SetCurrentTime(_curTime);
        eventStartBtns.Add(eventStartButton);
    }


    public IEnumerator PopupEvent(GameEvent gameEvent)
    {
        yield return null;
		if (!eventPopupUI.isOn && randomSelectPopupUI.gameObject.activeSelf == false)
		{
			AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.EventPopupOpen);
            eventPopupUI.ActivateEventPopUp();
            GameManager.Instance.TimeStop(true);
            GameManager.Instance.isTimeStoppedByEvent = true;
            GameManager.Instance.SetTimeControlPossibility(false);
            eventPopupUI.title.text = GameManager.GetLocalizingString(gameEvent.eventSOData.title, gameEvent.eventSOData.title_eng);
            eventPopupUI.context.text = GameManager.GetLocalizingString(gameEvent.eventSOData.context, gameEvent.eventSOData.context_eng);
            eventPopupUI.picture.sprite = gameEvent.eventSOData.sprite;
            eventPopupUI.ClearButtons();

            foreach (IGameEventButton gameEventButton in gameEvent.eventSOData.buttonDatas)
            {
                eventPopupUI.AddButton((GameEventButton)gameEventButton);
            }
        }
        else
        {
            StartCoroutine(PopupEvent(gameEvent));
        }
    }

    public void PopupRewardSelect(RewardSelect rewardSelect)
    {
        randomSelectPopupUI.gameObject.SetActive(true);
        GameManager.Instance.TimeStop(true);
        GameManager.Instance.isTimeStoppedByEvent = true;
        GameManager.Instance.SetTimeControlPossibility(false);
        randomSelectPopupUI.InitRandomSelectPopup(rewardSelect);
    }

    public void ReduceChanceSOTrainCar(SOTrainCar soTrainCar, float reduceChance = 1f)
    {
        for (int index = 0; index < soRandomRewardsList.Count; index++)
        {
            for (int rewardsIndex = 0; rewardsIndex < soRandomRewardsList[index].rewards.Count; rewardsIndex++)
            {
                if (((RewardTrainCar)(soRandomRewardsList[index].rewards[rewardsIndex].rewards[0])).soTrainCar.title.Equals(soTrainCar.title))
                {
                    soRandomRewardsList[index].rewards[rewardsIndex].chance -= reduceChance;

                    return;
                }
            }
        }
    }

    public Dictionary<SOEventData,float> SaveAgendaEvents()
    {
        curAgendaEvents.Clear();
        for (int i=0; i < eventStartBtns.Count; i++)
        {
            curAgendaEvents.Add(eventStartBtns[i].gameEvent.eventSOData, eventStartBtns[i].currentTime);
        }
        return curAgendaEvents;
    }

    #endregion

    #region PrivateMethod
    private void InitGameEvent()
    {
        for(int index = 0; index < initEvents.Count; index++)
        {
            RegisterGameEvent(initEvents[index]?.Clone());
        }
    }

    private void InitRandomRewards()
    {
        SORandomRewards[] soRandomRewardsArrayOrigin = Resources.LoadAll<SORandomRewards>("ScriptableObjects/Event/RandomRewards");
        for(int index = 0; index < soRandomRewardsArrayOrigin.Length; index++)
        {
            soRandomRewardsList.Add(soRandomRewardsArrayOrigin[index].Clone());
        }


    }

    #endregion
}
