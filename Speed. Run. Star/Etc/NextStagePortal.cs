using UnityEngine;
using DG.Tweening;

public class NextStagePortal : MonoBehaviour
{
    public Vector3 NextStageStartPosition;
    public GameObject portalEclipse;
    public Vector3 eclipseMaxScale;
    public Vector3 eclipseMinScale;
    public float fadeTime;
    public float fadeDelayTime;
    private GameObject player;
    private Sequence fadeSequence;

    private void Start()
    {
        fadeSequence = DOTween.Sequence()
            .Pause()
            .SetAutoKill(false)
            .Append(portalEclipse.transform.DOScale(eclipseMaxScale, fadeTime))
            .AppendInterval(fadeDelayTime)
            .OnComplete(() =>
            {
                TelePortPlayerToNext(player);
                portalEclipse.transform.position = player.transform.position;
                Camera.main.GetComponent<CameraController>().enabled = true;
                portalEclipse.transform.DOScale(eclipseMinScale, fadeTime);
                //portalEclipse.GetComponent<SpriteRenderer>().sortingOrder = 0;
            });

        
    }

    void OnTriggerEnter2D(Collider2D o)
    {
        if (o.gameObject.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>().enabled = false;
            player = o.gameObject;
            portalEclipse.GetComponent<SpriteRenderer>().sortingOrder = 5;
            portalEclipse.transform.localScale = eclipseMinScale;
            portalEclipse.SetActive(true);
            fadeSequence.Restart();
            //TelePortPlayerToNext(o.gameObject);
            //portalEclipse.transform.DOScale(eclipseMinScale, fadeTime);
            //fadeSequence.Rewind();
        }
    }

    void TelePortPlayerToNext(GameObject player)
    {
        player.transform.position = NextStageStartPosition;
    }

}
