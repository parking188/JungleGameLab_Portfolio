using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float power;
    public bool isRotate;
    Vector2 settingPosition;
    Rigidbody2D rigid;
    public void FireCannon()
    {
        settingPosition = transform.position;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        if(!isRotate)
        {
            rigid.velocity = new Vector2(-1, 0.6f) * power;
        }
        else
        {
            rigid.velocity = new Vector2(1, 0.6f) * power;
        }
        Invoke("ResetCannon", 5f);
    }

    void ResetCannon()
    {
        rigid.velocity = Vector2.zero;
        transform.position = settingPosition;
    }
}
