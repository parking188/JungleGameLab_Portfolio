using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicFakePlatform : MonoBehaviour
{
    public int timeZone;
    public GameObject platform;
    public Material defaultMat;
    public Material shiningMat;
    // Update is called once per frame
    void Update()
    {
        if (timeZone == 2)
        {
            for (int i = 0; i < platform.transform.childCount; i++)
            {
                if(platform.transform.GetChild(i).gameObject.GetComponent<BoxCollider2D>() != null)
                {
                    platform.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().material = shiningMat;
                }
            }
        }
        else
        {
            for (int i = 0; i < platform.transform.childCount; i++)
            {
                platform.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().material = defaultMat;
            }
        }
    }
}
