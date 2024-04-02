using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragObject : MonoBehaviour
{
	#region PublicVariables
	public SOTrainCar soTrainCar;
	public GameObject trainCar;
	public GameObject card;
    public GUICardInfo cardInfo;
    #endregion

    #region PrivateVariables
    private Image bodyImage;
    private SkeletonAnimation skeletonAnimation;
    public bool isActive { get; private set; }
    [SerializeField] private List<float> scaleByZoomLevel = new List<float>();
    private Vector2 defaultTrainCarOffset;
    #endregion

    #region PublicMethod
    public void InitDragObject()
	{
		if (trainCar == null) trainCar = transform.Find("TrainCar").gameObject;
		if (card == null) card = transform.Find("Card").gameObject;
        if (cardInfo == null) cardInfo = transform.GetComponentInChildren<GUICardInfo>();
        if (bodyImage == null) bodyImage = transform.Find("TrainCar").GetComponent<Image>();
        TurnOffDragObject();
        defaultTrainCarOffset = ((RectTransform)(trainCar.transform)).anchoredPosition;
    }

    public void InitDatas(SOTrainCar _soTrainCar)
    {
        soTrainCar = _soTrainCar;
        InitCardData();
        InitCarData();
    }

    public void TurnTrainCar()
    {
		AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.TrainPickup);
		trainCar.SetActive(true);
        card.SetActive(false);
        isActive = true;
    }

    public void TurnCard()
    {
        if (soTrainCar.isSecurityTrainCar) return;
		AudioManager.Instance.PlaySFXOneShot(AudioManager.ESoundType.CardSnap);
		trainCar.SetActive(false);
        card.SetActive(true);
        isActive = true;
    }

    public void TurnOffDragObject()
    {
        trainCar.SetActive(false);
        card.SetActive(false);
        isActive = false;
    }

    public void ScaleByCameraZoomLevel()
    {
        int currentZoomStep = GameManager.Instance.cameraController.TrainView.currentZoomStep;
        trainCar.transform.localScale = Vector3.Lerp(trainCar.transform.localScale, Vector3.one * scaleByZoomLevel[currentZoomStep], 10f * Time.unscaledDeltaTime);
        ((RectTransform)(trainCar.transform)).anchoredPosition = Vector2.Lerp(((RectTransform)(trainCar.transform)).anchoredPosition, 
            defaultTrainCarOffset * scaleByZoomLevel[currentZoomStep], 10f * Time.unscaledDeltaTime);
    }
    #endregion

    #region PrivateMethod
    private void Update()
    {
        ScaleByCameraZoomLevel();
    }

    private void InitCardData()
    {
        cardInfo.UpdateGUICardInfoData(soTrainCar);
    }

    private void InitCarData()
    {
        if (soTrainCar.skeletonDataAsset == null)
        {
            if (skeletonAnimation != null) Destroy(skeletonAnimation);
            bodyImage.sprite = soTrainCar.sprite;
            bodyImage.SetNativeSize();
        }
        else
        {
            if (skeletonAnimation != null) DestroyImmediate(skeletonAnimation);
            skeletonAnimation = SkeletonAnimation.AddToGameObject(trainCar.gameObject, soTrainCar.skeletonDataAsset);
            MeshRenderer meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "UI";
            meshRenderer.sortingOrder = 100;
            skeletonAnimation.state.SetAnimation(0, "animation", true);
        }
    }
    #endregion
}
