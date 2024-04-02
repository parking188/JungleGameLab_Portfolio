using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GUIEventPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	#region PublicVariables
	public Image dimmed;
	public Image frame;
	public TMP_Text title;
	public TMP_Text context;
	public Image picture;
    public RectTransform pictureMask;
    public bool isOn = false;
    [SerializeField] private Canvas canvas;
	[SerializeField] private GameObject buttons;
	[SerializeField] private GameObject buttonPrefab;
    #endregion

    #region PrivateVariables
    private CanvasGroup canvasGroup;
    private Sequence openingSequence;
    private Sequence closingSequence;
    private Coroutine closingCoroutine;
    private IEnumerator mouseHoverDetectCoroutine;
    #endregion

    #region PublicMethod
    public void AddButton(GameEventButton gameEventButton)
    {
        gameEventButton.InitGameEventButton();
        GUIEventButton button = Instantiate(buttonPrefab, buttons.transform).GetComponentInChildren<GUIEventButton>();
        button.context.text = GameManager.GetLocalizingString(gameEventButton.context, gameEventButton.context_eng);
        button.popupContext.text = GameManager.GetLocalizingString(gameEventButton.popupContext, gameEventButton.popupContext_eng);
        //button.popupObject.SetActive(true);
        //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)button.popupObject.transform.GetChild(0).transform) ;
        //LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)button.popupObject.transform.GetChild(0).GetChild(0).transform);
        //button.popupObject.SetActive(false);
        button.button.onClick.AddListener(() =>
        {
            if (!GameManager.Instance.isTimeStoppedByPlayer && !GameManager.Instance.isInventoryFull) GameManager.Instance.TimeStop(false);
            GameManager.Instance.isTimeStoppedByEvent = false;
            if (!GameManager.Instance.isInventoryFull) GameManager.Instance.SetTimeControlPossibility(true);
        });
        button.button.onClick.AddListener(gameEventButton.GiveRewards);
        //button.button.onClick.AddListener(ClearButtons);
        button.button.onClick.AddListener(() => { this.DisableEventPopUp(); });

        //���� �ٽ� ����
        button.button.onClick.AddListener(() =>
        {
            //ExploreMapManager.Instance.exploreMapTrainMovement.CheckNextPath();
        });

        if (gameEventButton.CheckConditions() == false)
        {
            button.button.interactable = false;
        }
    }
    #endregion

    #region PrivateMethod
    private void Start()
    {
        if (dimmed == null) dimmed = transform.Find("Dimmed").GetComponent<Image>();
        if (frame == null)      frame = transform.Find("PictureFrame").GetComponent<Image>();
        if (title == null)      title = transform.Find("Title").GetComponent<TMP_Text>();
        if (context == null)    context = transform.Find("Context").GetComponent<TMP_Text>();
        if (picture == null)    picture = transform.Find("Picture").GetComponent<Image>();
        if (buttons == null)    buttons = transform.Find("Buttons").gameObject;
        canvasGroup = GetComponent<CanvasGroup>();
        closingSequence = DOTween.Sequence();
        openingSequence = DOTween.Sequence();
    }

    public void ClearButtons()
    {
        GUIEventButton[] currentButtons = buttons.GetComponentsInChildren<GUIEventButton>();
        int length = currentButtons.Length;
        for(int index = 0; index < length; index++)
        {
            Destroy(currentButtons[index].gameObject);
        }
    }

    public void ActivateEventPopUp()
    {
        isOn = true;
        transform.Find("EventPopUpContent").gameObject.SetActive(true);

        StopAllCoroutines();
        if (openingSequence.IsActive())
        {
            openingSequence.Kill();
        }
        if (closingSequence.IsActive())
        {
            closingSequence.Kill();
        }

        openingSequence.Append(pictureMask.DOSizeDelta(new Vector2(999f, 430f), 1.0f).SetEase(Ease.InOutCirc).SetUpdate(true));
        dimmed.color = new Color(dimmed.color.r, dimmed.color.g, dimmed.color.b, 0.7f);
        
        canvasGroup.alpha = 1f;
        picture.GetComponent<RectTransform>().DOScale(Vector3.one, 2f).SetUpdate(true);
    }

    public void DisableEventPopUp()
    {
        if (isOn == true)
        {
            isOn = false;
            StopAllCoroutines();
            closingCoroutine = StartCoroutine(nextEventChecker());
        }
    }


    IEnumerator nextEventChecker()
    {
        yield return new WaitForEndOfFrame();
        AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.ExploreTransition);

        if (closingSequence != null && closingSequence.IsActive())
        {
            closingSequence.Kill();
        }
        closingSequence = DOTween.Sequence();
        Tween fadeAnimation = dimmed.DOFade(0f, 0.5f);
        Tween sizeAnimation = pictureMask.DOSizeDelta(new Vector2(997f, 0f), 0.5f).SetEase(Ease.InOutCirc);
        Tween canvasFadeAnimation = canvasGroup.DOFade(0f, 0.2f);
        closingSequence.Join(fadeAnimation)
                  .Join(sizeAnimation)
                  .Join(canvasFadeAnimation);
        closingSequence.OnComplete(() =>
        {
            if (isOn == false)
            {
                canvasGroup.alpha = 0f;
                transform.Find("EventPopUpContent").gameObject.SetActive(false);
            }

            picture.GetComponent<RectTransform>().localScale = Vector3.one * 3;
            openingSequence.Kill();
            openingSequence = DOTween.Sequence();
            closingSequence.Kill();
            closingSequence = DOTween.Sequence();
        }).SetUpdate(true);
        closingSequence.Play();
        yield return closingSequence.WaitForCompletion();
        yield return new WaitForEndOfFrame();
        pictureMask.sizeDelta = new Vector2(997f, 0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {/*
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        Debug.Log(mousePosition);
        var linkedTaggedText = TMP_TextUtilities.FindIntersectingLink(context, mousePosition, canvas.worldCamera);

        if (linkedTaggedText != -1)
        {
            TMP_LinkInfo linkInfo = context.textInfo.linkInfo[linkedTaggedText];
            Application.OpenURL(linkInfo.GetLinkID());
        }
		*/
    }


    private IEnumerator MouseHoverDetectCoroutine()
    {
        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //mouseHoverDetectCoroutine = StartCoroutine(MouseHoverDetectCoroutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    private void OnDestroy()
    {
        if (closingSequence != null)
            closingSequence.Kill();
    }
    #endregion
}
