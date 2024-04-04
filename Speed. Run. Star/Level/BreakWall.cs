using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Player>().stateMachine.CurrentState.GetType() == typeof(DashState))
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
                SetActiveDelayed(0.5f, false);
            }
        }
    }


    private IEnumerator SetActiveDelayed(float duration, bool setActive) {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(setActive);
    }


    public void OnReset()
    {
        gameObject.SetActive(true);
    }

}
