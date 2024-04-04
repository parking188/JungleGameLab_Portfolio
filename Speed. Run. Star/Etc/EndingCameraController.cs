using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndingCameraController : MonoBehaviour
{
    public GameObject cameraPosition;
    public float targetCameraSize;

    public void EndingCameraWork()
    {
        Camera.main.GetComponent<CameraController>().enabled = false;
        transform.position = new Vector3(cameraPosition.transform.position.x, cameraPosition.transform.position.y, -10);
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, targetCameraSize, 0.8f).SetEase(Ease.InExpo);
    }
}
