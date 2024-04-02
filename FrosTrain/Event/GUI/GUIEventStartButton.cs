using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIEventStartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	#region PublicVariables
	[HideInInspector] public GameEvent gameEvent;
	public float alarmExpireTime { get; private set; }
	public float currentTime { get; private set; }

	private Vector2 originalScale;
	private Image image;
    private bool isClosing = false;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void InitEventStartButton(GameEvent _gameEvent)
	{
		gameEvent = _gameEvent;
		alarmExpireTime = gameEvent.eventSOData.alarmExpireTime;
		currentTime = 0f;

        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
           
            if (GameEventManager.Instance.CheckActiveEvent() == false)
            {
                isClosing = true;
                GameEventManager.Instance.eventStartBtns.Remove(this);
                gameEvent.OccurGameEvent();
			transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo).SetUpdate(true).OnComplete(() => Destroy(this.gameObject));
	}
        });
    }
    #endregion

    #region PrivateMethod

    private void Start()
    {
        image = transform.Find("Btn_Agenda_Gauge").GetComponent<Image>();
        StartPunchTween();
    }
    private void Update()
    {
		currentTime += Time.deltaTime;
		image.fillAmount = 1-currentTime / alarmExpireTime;
		if(currentTime >= alarmExpireTime)
        {
            GameEventManager.Instance.eventStartBtns.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isClosing)
        {
			transform.DOScale(1f, 0.5f);
		}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isClosing)
        {
            StartPunchTween();
        }
    }

    private void StartPunchTween()
    {
		transform.DOScale(0.85f, 0.5f).From(0.9f).SetUpdate(true).SetLoops(int.MaxValue);
	}

    public void SetCurrentTime(float _time)
    {
        currentTime = _time;
    }
	#endregion
}
