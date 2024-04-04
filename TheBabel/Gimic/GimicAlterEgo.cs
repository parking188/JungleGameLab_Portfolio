using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicAlterEgo : MonoBehaviour
{
    public GameObject origin;
    GameObject alterEgo;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            alterEgo = Instantiate(origin);
            alterEgo.transform.position = new Vector2(origin.transform.position.x, origin.transform.position.y + 11);
        }
    }
}
