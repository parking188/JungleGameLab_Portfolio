using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;

public class GUIGameOverPopup : MonoBehaviour
{
    public GUIButton exitBtn;
    public bool isFail;
    private RectTransform bgRectTransform;
    private RectTransform frameTransform;
    private TMP_Text TitleText;
    private TMP_Text DescText;
    private TMP_Text ButtonText;

    [SerializeField]private GUIGameClearRewardPopup rewardPopup;
    
    #region PublicVariables
    #endregion
    #region PrivateVariables
    #endregion
    #region PublicMethod
    public void Print()
    {
        GameManager.Instance.isEnding = true;
        GameManager.Instance.TimeStop(true);
        GameManager.Instance.SetTimeControlPossibility(false);
        GameManager.Instance.isTutorialTrainA = true;
        transform.localScale = Vector3.one;
        frameTransform.anchoredPosition = new Vector2(frameTransform.anchoredPosition.x, - frameTransform.rect.height);
        frameTransform.DOAnchorPosY(frameTransform.rect.height/2, 1f).SetUpdate(true); 
        bgRectTransform.DOScale(Vector3.one, 2f).From(Vector3.one * 2f).SetEase(Ease.OutQuart).SetUpdate(true);

        if (isFail)
        {
            ExploreManager.Instance.guiExplorePanel.TurnAlarmOff();
            AudioManager.Instance.StopAmbience();
            AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.EventPopup);
            TitleText.text = GameManager.GetLocalizingString("실패하였습니다.", "Failed");
            DescText.text = GameManager.GetLocalizingString("당신은 승객들의 기대를 저버렸습니다.\n" + GameManager.Instance.timeSystem.CurrentDate +"일차", "You've let your passengers down.\n" + "Day : " + GameManager.Instance.timeSystem.CurrentDate);
            ButtonText.text = GameManager.GetLocalizingString("돌아가기", "MainMenu");
            SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_06);
        }
        else
        {
            GameManager.Instance.CheckGameClearCount();
            int count = SaveManager.Instance.gameClearCount;
            if (count <= 4)
            {
                exitBtn.onClicked.RemoveAllListeners();
                exitBtn.onClicked.AddListener(PrintRewardPopup);
            }

            ExploreManager.Instance.guiExplorePanel.TurnAlarmOff();
            AudioManager.Instance.StopAmbience();
            AudioManager.Instance.ChangeBGM(Environment.plain);
            TitleText.text = GameManager.GetLocalizingString("클리어", "Stage Clear");
            DescText.text = GameManager.GetLocalizingString("승객들은 이 날을 기억할 것입니다." + "\n점수 : " + ResourceManager.Instance.GetHappiness().Value , "Passengers will remember this day." + "\nPoints : " + ResourceManager.Instance.GetHappiness().Value);
            ButtonText.text = GameManager.GetLocalizingString("돌아가기", "MainMenu");

            CheckClearAchievement();
        }
    }

    public void CheckClearAchievement()
    {
        // 37일 이전 승리
        if (GameManager.Instance?.timeSystem.CurrentDate < AchievementData.ACHIEVEMENT_INT_SUPERRAPIDTRAIN)
        {
            SaveManager.Instance?.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_20);
        }

        // 행복도 2만 이상 승리
        if (ResourceManager.Instance.GetHappiness().Value >= 20000)
        {
            SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_16);
        }

        // 각 기관사별 승리
        switch (SaveManager.Instance.trainDriver)
        {
            case ETrainDriver.Lim:
                SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_07);
                break;
            case ETrainDriver.Choi :
                SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_08);
                break;
            case ETrainDriver.Cho:
                SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_15);
                break;
        }

        // 통신 시너지 활성화 승리
        if (GameManager.Instance.player.train.carTagCountList[(int)ECarTag.Communication] >= AchievementData.ACHIEVEMENT_INT_OLDFOE)
        {
            SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_11);
        }

        // 오락 시너지 2레벨 활성화 승리
        if (GameManager.Instance.player.train.carTagCountList[(int)ECarTag.Entertainment] >= AchievementData.ACHIEVEMENT_INT_ADDICT)
        {
            SaveManager.Instance.TriggerAchievement(AchievementData.ACHIEVEMENT_STRING_13);
        }
    }

    public void PrintRewardPopup()
    {
        exitBtn.SetInteractable(false);
        frameTransform.DOAnchorPosY(-frameTransform.rect.height / 2, 0.5f).SetUpdate(true).OnComplete(()=>
        {
            //rewardPopup.DroneTransition();
            rewardPopup.gameObject.SetActive(true);
        });
    }

    public void Erase()
    {
        transform.localScale = Vector3.zero;
    }

    #endregion
    #region PrivateMethod
    private void Start()
    {
        transform.localScale = Vector3.zero;
        if (exitBtn == null) exitBtn = transform.Find("Frame/Exit_Button").GetComponent<GUIButton>();
        if (frameTransform == null) frameTransform = transform.Find("Frame").GetComponent<RectTransform>();

        exitBtn.onClicked.AddListener(GoToMain);
        bgRectTransform = (RectTransform)(transform.Find("Image/Background"));
        TitleText = frameTransform.Find("Title").GetComponent<TMP_Text>();
        DescText = frameTransform.Find("Context").GetComponent<TMP_Text>();
        ButtonText = exitBtn.GetComponentInChildren<TMP_Text>();
    }

    private void GoToMain()
    {
        exitBtn.SetInteractable(false);
        GameManager.Instance.TimeStop(false);
        GameManager.Instance.SetTimeControlPossibility(true);
        GameManager.Instance.GoToMain();
    }

    private void OnDestroy()
    {
        if(bgRectTransform != null)
        {
            bgRectTransform.DOKill();
        }
        if(frameTransform != null)
        {
            frameTransform.DOKill();
        }
    }
    #endregion
}