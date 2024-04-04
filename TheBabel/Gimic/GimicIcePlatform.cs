using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicIcePlatform : MonoBehaviour
{
    int timeZone = 1;
    float meltSpeed = 0.025f;
    // Update is called once per frame
    private void Start()
    {
        if (timeZone != 2)
        {
            StartCoroutine(MetingIce());
        }
    }
    IEnumerator MetingIce()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - meltSpeed);
            if(transform.localScale.y <= 0 )
            {
                GetComponent<BoxCollider2D>().enabled = false;
                yield break;
            }
        }
    }
}
