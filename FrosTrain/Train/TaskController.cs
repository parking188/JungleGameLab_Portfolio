using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskController : MonoBehaviour
{
    #region PublicVariables
    public SpriteRenderer spriteRenderer;
    public bool isClosing = false;
    public TrainCar trainCar;
    public SOTask taskSO;
    #endregion

    #region PrivateVariables
    private float lastingTime;
    private MouseAction mouseAction;
    #endregion

    #region PublicMethod
    public void ShowTask(Sprite sprite, float lastingTime)
    {
        gameObject.SetActive(true);
        transform.DOScale(1f, 0.75f).From(1.25f).SetLoops(-1).SetUpdate(true);
        spriteRenderer.sprite = sprite;
        this.lastingTime = lastingTime;
        StartCoroutine(DeactivateAfterDelay());
    }

    private void InitMouseAction()
    {
        mouseAction = GetComponent<MouseAction>();
        mouseAction.onMouseDownLeft = MouseDown;
    }

    public void CloseTask()
    {
        isClosing = true;
        transform.DOKill();
        Tween scalingUp = transform.DOScale(1.5f, 0.25f);
        Tween fadeOut = transform.DOScale(0f, 0.1f);

        Sequence closingSequence = DOTween.Sequence();
        closingSequence.Join(scalingUp).Append(fadeOut).OnComplete(() => {
            gameObject.SetActive(false);
            lastingTime = float.MaxValue;
            spriteRenderer.sprite = null;
            StopAllCoroutines();
            isClosing = false;
        }).SetUpdate(true);
    }
    #endregion

    #region PrivateMethod
    #endregion

    private void Start()
    {
        InitMouseAction();
    }
    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(lastingTime);
        lastingTime = float.MaxValue;
        spriteRenderer.sprite = null;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (transform != null)
            transform.DOKill();
    }

    public void MouseDown(PointerEventData eventData)
    {
        if (trainCar.currentSOTask != null && !isClosing)
        {
            ResourceManager.Instance?.GetHappiness().AddValue(trainCar.currentSOTask.reward);
            EffectManager.Instance?.InstantiateCounterText(transform.position, trainCar.currentSOTask.reward);
            EffectManager.Instance?.PlayOneShot(EffectManager.EParticleEffectType.happinessBomb, transform.position, trainCar.currentSOTask.reward);

            CloseTask();
            if (!SaveManager.Instance.isTuto) SaveManager.Instance.AddSteamUserStat(AchievementData.ACHIEVEMENT_STRING_09);
        }
    }
}