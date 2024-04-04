using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Spider : MonoBehaviour, IEnemy
{
    [Header("Behave")]
    public float updownRatio = 0.8f;
    public float updownHeight = 5.0f;
    public float updownPeriod = 2.0f;

    [Header("Attack")]
    public float knockbackForce = 1000.0f;

    private SpriteShapeController spiderLine;
    private Vector3 ceilingPosition;
    private Vector3 descentPosition;
    private Vector3 ceilingPoint;
    private bool isDescenting = true;

    // Start is called before the first frame update
    void Start()
    {
        spiderLine = GetComponentInChildren<SpriteShapeController>();
        ceilingPosition = transform.position;
        descentPosition = transform.position;
        descentPosition.y -= updownHeight;
        ceilingPoint = spiderLine.spline.GetPosition(0);
        InvokeRepeating("SwitchDescenting", updownPeriod, updownPeriod);
    }

    // Update is called once per frame
    void Update()
    {
        Behave();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.Instance.player;
            player.KnockBack(collision.contacts[0].normal * -1.0f, knockbackForce);
            Attack(player);
        }
    }

    public void Attack(IDamageable damageable)
    {
        damageable.Damage(1);
    }

    public void Behave()
    {
        float moveHeightForLine = transform.position.y;

        if(isDescenting)
        {
            transform.position = Vector3.Lerp(transform.position, descentPosition, updownRatio * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ceilingPosition, updownRatio * Time.deltaTime);
        }

        moveHeightForLine -= transform.position.y;
        ceilingPoint.x += moveHeightForLine;
        spiderLine.spline.SetPosition(0, ceilingPoint);
    }

    private void SwitchDescenting()
    {
        isDescenting = !isDescenting;
    }
}
