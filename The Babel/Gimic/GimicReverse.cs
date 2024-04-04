using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GimicReverse : MonoBehaviour
{
    public GameObject flatform;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("상하반전");
            for(int i = 0; i < flatform.transform.childCount; i++)
            {
                flatform.transform.GetChild(i).gameObject.transform.position = new Vector2 (flatform.transform.GetChild(i).gameObject.transform.position.x, flatform.transform.GetChild(i).gameObject.transform.position.y * -1);
            }
        }
    }
}
