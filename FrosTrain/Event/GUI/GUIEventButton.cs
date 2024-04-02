using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine;

[RequireComponent(typeof(MouseAction))]
public class GUIEventButton : MonoBehaviour
{
    #region PublicVariables
    public Image image;
    public Button button;
	public TMP_Text context;
    public TMP_Text popupContext;
    public GameObject popupObject;
    #endregion

    #region PrivateVariables
    [SerializeField] private MouseAction mouseAction;
    #endregion

    #region PublicMethod
    #endregion

    #region PrivateMethod
    private void Start()
    {
        if (image == null)          image = GetComponent<Image>();
        if (button == null)         button = GetComponent<Button>();
        if (context == null)        context = transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        if (popupContext == null)   popupContext = transform.Find("PopupContext").GetComponent<TMP_Text>();
        if (popupObject == null)    popupObject = transform.Find("Popup").gameObject;
        if (mouseAction == null)    mouseAction = GetComponent<MouseAction>();

        
        mouseAction.onMouseEnter += (eventData) => {
            if (popupContext.text != "")
            {
                popupObject.SetActive(true);
                popupObject.transform.SetParent(transform.parent.parent, true);
            }
        };
        mouseAction.onMouseExit += (eventData) => {
            if (popupContext.text != "")
            {
                popupObject.SetActive(false);
            }
        };
    }

    private void OnDestroy()
    {
        Destroy(popupObject);
    }
    #endregion
}
