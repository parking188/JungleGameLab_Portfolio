using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimicCannonNiddle : MonoBehaviour
{
    public float power;
    public float rotateSpeed = 30f;

    Coroutine rotateNiddle;
    public Coroutine followNiddle;

    Vector2 originPos;
    Rigidbody2D rigid;
    GameObject player;
    bool isFireReady = false;
    private void Start()
    {
        originPos = transform.position;
        rigid = GetComponent<Rigidbody2D>();
        rotateNiddle = StartCoroutine(RotateNiddle());
        player = GameManager.Instance.player.gameObject;
    }

    private void Update()
    {
        if (isFireReady)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireCannon();
            }
        }
        else
        {
            if (followNiddle != null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ResetCannon();
                }
            }
        }
    }
    public void ReadyCannon()
    {
        isFireReady = true;
        rigid.velocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        followNiddle = StartCoroutine(FollowNiddle());
    }
    void FireCannon()
    {
        StopCoroutine(rotateNiddle);
        rigid.AddRelativeForce(Vector3.up*power, ForceMode2D.Impulse);
        isFireReady = false;
    }

    void ResetCannon()
    {
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        StopCoroutine(followNiddle);
        rigid.velocity = Vector3.zero;
        transform.position = originPos;
        Invoke("resetTrigger", 5f);
    }
    void resetTrigger()
    {
        GetComponentInParent<CircleCollider2D>().enabled = true;
        rotateNiddle = StartCoroutine(RotateNiddle());
    }
    IEnumerator RotateNiddle()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            transform.Rotate(Vector3.forward * rotateSpeed);
        }
    }
    IEnumerator FollowNiddle()
    {
        while (true)
        {
            yield return null;
            player.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Platform"))
        {
            ResetCannon();
        }
    }
}
