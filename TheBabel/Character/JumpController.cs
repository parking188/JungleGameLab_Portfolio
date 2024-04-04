using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{

    public RaycastHit2D rayHit;
    private float fJumpPower = 1000.0f;
    private Vector2 jumpRay = Vector2.down; //점프의 반대방향으로 발사
    public bool jumpNaturallyWhenInversed = false;
    public float JumpPower
    {
        get
        {
            return fJumpPower;
        }
    }
    private Rigidbody2D rbPlayer;
    private CameraController cameraController;

    [SerializeField]
    private bool isJumping;
    public bool IsJumping
    {
        get
        {
            return isJumping;
        }
    }

    public void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        CheckJumping();
        CheckInversed(); 
    }

    private void CheckJumping()
    {
        if (jumpRay.y<0&&rbPlayer.velocity.y <= 0
            ||jumpRay.y>0&&rbPlayer.velocity.y >=0)
        {
            Debug.DrawRay(rbPlayer.position, Vector2.down, new Color(0, 1, 0));
            rayHit = Physics2D.Raycast(rbPlayer.position, jumpRay, 1.3f, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                isJumping = false;


                if (rayHit.collider.tag == "Gate")
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        rayHit.collider.GetComponent<GateBlock>().UseGate();
                    }
                }
                if (rayHit.collider.tag == "Cannon")
                {
                    rayHit.collider.GetComponent<Cannon>().FireCannon();
                }
                if(rayHit.collider.CompareTag("FallingPlatform"))
                {
                    rayHit.collider.gameObject.GetComponent<GimicFallingPlatform>().Falling();
                }
            }
            else
            {
                isJumping = true;
            }
        }
    }

    void CheckInversed()
    {
        if (jumpNaturallyWhenInversed)
        {
            if(cameraController == null)
            {
                cameraController = Camera.main.gameObject.GetComponent<CameraController>();
            }

            if (cameraController != null 
                && Camera.main.gameObject.GetComponent<CameraController>().fZOffset < 0.0f)
            {
                //정상
                jumpRay = Vector2.down;
            }
            else
            {
                jumpRay = Vector2.up;
            }
        }
    }

    public void Jump()
    {
        Jump(fJumpPower);
    }
    public void Jump(float jumpPower, float jumpDirX = 0.0f, float jumpDirY = 1.0f)
    {
        isJumping = true;
        rbPlayer.velocity *= new Vector3(1.0f, 0.0f, 1.0f);
        Vector2 force = (new Vector2(jumpDirX, jumpDirY)).normalized * jumpPower;
        rbPlayer.AddForce(force, ForceMode2D.Impulse);
    }
    
}
