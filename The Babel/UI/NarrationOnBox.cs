using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationOnBox : MonoBehaviour
{
    [TextArea]
    public string narrationText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameManager.Instance.ShowNarration(narrationText);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
