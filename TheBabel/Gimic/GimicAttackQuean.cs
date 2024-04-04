using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicAttackQuean : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            if (collision.gameObject.transform.localScale == new Vector3(5, 5, 5))
            {
                Debug.Log("Å¬¸®¾î");
            }
        }
    }
}
