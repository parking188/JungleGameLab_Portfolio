using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIdleMover : MonoBehaviour
{
    //특정 거리 왔다갔다함.
    public Vector2 direction;
    public float moveSpeed;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    float counter = 0.0f;
    float delta = -1.0f;
    void Update()
    {
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);

        counter += moveSpeed * Time.deltaTime * delta;
        if (counter >= distance||counter<=0.0f)
        {
            direction *= -1.0f;
            delta *= -1.0f;
        }
    }

}
