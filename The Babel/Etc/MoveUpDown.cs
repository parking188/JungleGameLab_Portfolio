using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpDown : MonoBehaviour
{
    public float fMoveSpeed = 2.0f;
    public float fMoveBound = 1.0f;
    private Vector3 v3DefaultPos;
    private bool bMoveUp;

    // Start is called before the first frame update
    void Start()
    {
        v3DefaultPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpAndDown();
    }

    private void MoveUpAndDown()
    {
        transform.position += new Vector3(0.0f, (bMoveUp ? fMoveSpeed : -fMoveSpeed) * Time.deltaTime, 0.0f);

        if(transform.position.y > v3DefaultPos.y + fMoveBound)
        {
            bMoveUp = false;
        }
        else if(transform.position.y  < v3DefaultPos.y - fMoveBound)
        {
            bMoveUp = true;
        }
    }
}
