using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable, IKnockBackable
{
    [Header("Player Movement")]
    public float fSpeed = 10.0f;
    public float fJumpPower = 1000.0f;
    private float originalJumpPower = 1400.0f;
    [SerializeField] private int nMaxJumpCount = 2;
    private int nJumpCount;
    public int JumpCount
    {
        get
        {
            return nJumpCount;
        }
    }

    private float defaultSpeed;

    [Header("Player Status")]
    [SerializeField] private int nMaxHp = 3;
    private int nCurHp = 3;
    public int GetMaxHp() { return nMaxHp; }
    public int GetCurHp() { return nCurHp; }
    private bool bIsInvincible = false;
    public bool GetIsInvincible() { return bIsInvincible; }
    private bool bIsControllable = true;
    public bool IsInvincible
    {
        get { return bIsInvincible; }
        set { 
            //True일 경우 이펙트 입히기
            bIsInvincible = value; 
        }
    }
    //글라이더 등 일부 활동에서는 넉백 불가능, 해당 사항 변수
    private bool bIsKnockbackable = true;
    public bool IsKnockbackable
    {
        get { return bIsKnockbackable; }
        set { bIsKnockbackable = value; }
    }

    private Rigidbody2D rbPlayer;
    private CapsuleCollider2D colliderPlayer;
    private Material matPlayer;
    private Coroutine coroutineDamaged;
    [HideInInspector] public JumpController jumpController;

    public bool isSliding = false;
    private float vignetteIntensityDamaged = 0.3f;
    public float originalMass = 70.0f;
    public float originalGravityScale = 5.0f;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        defaultSpeed = fSpeed;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        MovePlayer();
        JumpPlayer();
        CheckCanJump();
    }

    private bool oldJumping;
    void CheckCanJump()
    {
        if (oldJumping&&!jumpController.IsJumping)
        {
            ResetJump();
        }
        oldJumping = jumpController.IsJumping;
    }

    private void Init()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        colliderPlayer = GetComponent<CapsuleCollider2D>();
        matPlayer = GetComponent<SpriteRenderer>().material;
        jumpController = GetComponent<JumpController>();
        ResetHp();
        ResetSpeed();
        JumpCountReset();
    }

    public void ResetHp()
    {
        nCurHp = nMaxHp;
        GameManager.Instance.TurnOnAllHpImage();
    }

    public void ResetSpeed()
    {
        fSpeed = defaultSpeed;

        if(rbPlayer == null)
        {
            rbPlayer = GetComponent<Rigidbody2D>();
        }

        if(colliderPlayer == null)
        {
            colliderPlayer = GetComponent<CapsuleCollider2D>();
        }

        GetComponent<ItemManager>().GetComponentInChildren<GliderItem>().StopGliding();

        rbPlayer.mass = originalMass;
        rbPlayer.gravityScale = originalGravityScale;
        fJumpPower = originalJumpPower;
        colliderPlayer.isTrigger = false;
    }

    private void MovePlayer()
    {
        float fHorizontal = Input.GetAxis("Horizontal");
        Vector2 v2HorizontalVelocity = new Vector2(fHorizontal * fSpeed, rbPlayer.velocity.y);
        if(bIsControllable && !isSliding)
        {
            rbPlayer.velocity = v2HorizontalVelocity;
        }
    }


    private void JumpPlayer()
    {
        if (Input.GetButtonDown("Jump") )
        {
            if (jumpController.IsJumping && nMaxJumpCount == nJumpCount)
            {
                //nJumpCount가 닳지 않았다는 것은 떨어졌다는 뜻이므로 점프 사용 불가
            }
            else if (nJumpCount > 0)
            {
                nJumpCount--;
                jumpController.Jump(fJumpPower);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Platform"))
        //{
        //    JumpCountReset();
        //}
        if (collision.gameObject.CompareTag("Danger"))
        {
            Kill();
        }
    }


    void ResetJump()
    {
        JumpCountReset();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Healing"))
        {
            HealPlayer();
            Destroy(collision.gameObject);
        }
    }

    public void JumpCountReset()
    {
        nJumpCount = nMaxJumpCount;
    }

    public IEnumerator InInvincible()
    {
        int nMaxBlink = 5;
        int nBlinkCnt = 0;
        float fInvincibleTime = 0.5f;
        float fBlinkTime = (fInvincibleTime / nMaxBlink) / 2.0f;

        Color colorOriginal = matPlayer.color;
        Color colorDamaged = Color.red;

        bIsInvincible = true;
        bIsControllable = false;

        while (nBlinkCnt < nMaxBlink)
        {
            nBlinkCnt++;
            matPlayer.SetColor("_BaseColor", colorDamaged);
            yield return new WaitForSeconds(fBlinkTime);
            matPlayer.SetColor("_BaseColor", colorOriginal);
            yield return new WaitForSeconds(fBlinkTime);
        }

        bIsInvincible = false;
        bIsControllable = true;
    }

    public void Kill()
    {
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }

    public void Damage(int damage)
    {
        if (bIsInvincible)
        {
            return;
        }

        float vignetteTime = 0.4f;
        StartCoroutine(GameManager.Instance.cameraController.cameraShake());
        StartCoroutine(GameManager.Instance.SetVignetteIntensity(vignetteIntensityDamaged, vignetteTime));
        GameManager.Instance.TurnOffHpImage(nCurHp);
        nCurHp--;

        if (nCurHp < 1)
        {
            Kill();
        }
        else
        {
            coroutineDamaged = StartCoroutine(InInvincible());
        }
    }

    // 넉백기능 수정 필요
    public void KnockBack(Vector3 direction, float force)
    {
        if (bIsInvincible||!IsKnockbackable)
        {
            return;
        }

        direction.Normalize();
        rbPlayer.velocity = Vector2.zero;
        rbPlayer.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void HealPlayer()
    {
        if (nCurHp >= nMaxHp)
        {
            return;
        }

        nCurHp++;
        GameManager.Instance.TurnOnHpImage(nCurHp);
    }


}
