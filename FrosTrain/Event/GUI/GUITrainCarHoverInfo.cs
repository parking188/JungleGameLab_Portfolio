using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GUITrainCarHoverInfo : MonoBehaviour
{
    #region PublicVariables
    public GUICardInfo cardInfo;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void InitTrainCarHoverInfo()
    {
        if (cardInfo == null) cardInfo = GetComponent<GUICardInfo>();
        cardInfo.InitGUICardInfo();
    }

    public void UpdateCardPosition(TrainCar trainCar)
    {
        Vector3 worldPosition = WorldToCanvasPosition((RectTransform)(transform.parent), Camera.main, trainCar.transform.position + Vector3.up * trainCar.transform.localScale.y);
        
        /*
        Vector3 worldPosition = trainCar.transform.position;
        worldPosition.y = Screen.height * MainCanvasController.Instance.transform.localScale.y * 0.3f;
        worldPosition.z = cardInfo.transform.position.z;
        */
        ((RectTransform)(cardInfo.transform)).anchoredPosition = worldPosition;
        cardInfo.UpdateSynergyInfoPosition(true);
        if (cardInfo.gameObject.activeSelf == false)
        {
            cardInfo.ShowSynergyInfoList(trainCar.soTrainCar);
            cardInfo.gameObject.SetActive(true);
        }
    }

    private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        Vector2 temp = camera.WorldToViewportPoint(position);
        //Calculate position considering our percentage, using our canvas size
        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;
        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }

    public void NullifyInfoData()
    {
        cardInfo.NullifyInfoData();
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();
    }
    #endregion

    #region PrivateMethod
    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.transform));
    }

    private void OnDisable()
    {
        MainCanvasController.Instance.guiTrainCarHoverSynergyInfo.HideAllSynergyInfo();
    }
    #endregion
}
