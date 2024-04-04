using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Player player;
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public PlayerGround ground;
    
    // effect
    public GameObject playerSprite;
    [HideInInspector] public Animator animator;
    Vector3 initPlayerLocalScale;
    public ParticleSystem BoostEffectRight;
    public ParticleSystem BoostEffectLeft;

    [HideInInspector] public SpriteRenderer spriteRenderer;


    public PlayerRigidbodyData rigidData;

    public InputData input;



    void Awake()
    {
        player = GetComponent<Player>();
        player.SetController(this);
        rigid = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerGround>();
        input = new InputData();

        animator = playerSprite.GetComponent<Animator>();
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        initPlayerLocalScale = playerSprite.transform.parent.transform.localScale;
    }

    private void Start()
    {
        InitRigidData();
    }

    public void InitRigidData()
    {
        rigidData.blockSize = 8f;
        rigidData.runAcc = 0f;
        rigidData.runDeAcc = 0f;
        rigidData.jumpTimer = 0f;
        rigidData.desiredJump = false;
        rigidData.isJumping = false;
        rigidData.isAirJumping = false;

        rigidData.dashTimer = 0f;
        rigidData.dashBufferTimer = 0f;
        rigidData.dashDirection = new Vector2(1, 0);
        rigidData.runSpeed = rigidData.runSpeedPerBlock * rigidData.blockSize;
        rigidData.runAcc = rigidData.runSpeed * rigidData.runDirection / rigidData.runAccelerationTime;
        rigidData.runDeAcc = rigidData.runSpeed * rigidData.runDirection / rigidData.runDecelerationTime * -1;

        rigidData.jumpHeight = rigidData.jumpHeightPerBlock * rigidData.runSpeedPerBlock;
        rigidData.jumpUpAcc = -2 * rigidData.jumpHeight / (rigidData.jumpUpTime * rigidData.jumpUpTime) - rigidData.gravity.y;
        rigidData.jumpStartVel = 2 * rigidData.jumpHeight / rigidData.jumpUpTime;

        rigidData.dashDistance = rigidData.dashDistanceBlock * rigidData.blockSize;
        rigidData.dashVel = rigidData.dashDistance / rigidData.dashTime;
        rigidData.isDashing = false;

        rigidData.boostTimer = 0f;
        rigidData.isBoosting = false;
        rigidData.isLandingBoost = false;

        rigidData.springTimer = 0f;

        rigidData.trampolineTimer = 0f;
        rigidData.isTrampolining = false;

        rigidData.springTimer = 0f;
        rigidData.isSpring = false;

        rigidData.runVelocity = Vector2.zero;
        rigidData.boostVelocity = Vector2.zero;
        rigidData.jumpVelocity = Vector2.zero;
        rigidData.dashVelocity = Vector2.zero;
        rigidData.springVelocity = Vector2.zero;
        rigidData.trampolineVelocity = Vector2.zero;
        rigidData.tmpVelocity = Vector2.zero;
    }

    private void Update()
    {
        PlayerInputCustom.Instance.GetInput(out input);
        animator.SetFloat("VerticalAxisValue", input.directionY);
        if(input.directionX < 0){
            playerSprite.transform.parent.transform.localScale = new Vector3(-1 *initPlayerLocalScale.x , initPlayerLocalScale.y, initPlayerLocalScale.z);
        }
        else if(input.directionX > 0){
            playerSprite.transform.parent.transform.localScale = initPlayerLocalScale;
        }

        if (((input.buttonsDown & InputData.LANDBUTTON) == InputData.LANDBUTTON) && ground.GetOnLandingDashGround())
        {
            rigidData.isLandingBoost = true;
            animator.SetBool("isBoost", true);
        }

    }

    private void FixedUpdate()
    {
        if(rigidData.isDashBufferTime){
            rigidData.dashBufferTimer += Time.fixedDeltaTime;
            if(rigidData.dashBufferTimer > rigidData.dashBufferTime){
                rigidData.isDashBufferTime = false;
                rigidData.dashBufferTimer = 0;
            }
        }

        if(ground.GetOnGround()){
            rigidData.tmpVelocity = Vector2.zero;
        }
        CalculateVelocity();
    }

    void CalculateVelocity()
    {
        rigidData.tmpVelocity += rigidData.gravity;
        
        rigid.velocity = rigidData.runVelocity 
            + rigidData.boostVelocity 
            + rigidData.jumpVelocity 
            + rigidData.dashVelocity 
            + rigidData.springVelocity
            + rigidData.trampolineVelocity
            + rigidData.tmpVelocity;

        // Debug.Log(rigid.velocity + " " +rigidData.trampolineVelocity);
        // physics
        if (rigid.velocity.y < rigidData.airTerminalVelocity)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigidData.airTerminalVelocity);
        }
    }

    public void SetBoostOn(float _boostVelocityPerBlock, float _boostTime, float _boostAccelerationTime, float _boostDecelerationTime){
        rigidData.boostVelocityPerBlock = _boostVelocityPerBlock;
        rigidData.boostTime = _boostTime;
        rigidData.boostAccelerationTime = _boostAccelerationTime;
        rigidData.boostDecelerationTime = _boostDecelerationTime;
        rigidData.isBoosting = true;
    }

    
    public void SetTrampolineOn(Vector2 _trampolineDirection, float _trampolineTime, float _trampolineDistance){
        rigidData.trampolineDirection = _trampolineDirection;
        rigidData.trampolineTime = _trampolineTime;
        rigidData.trampolineDistance = _trampolineDistance;
        rigidData.isTrampolining = true;
    }

    public void SetSpringOn(float _springHeigtBlock, Vector2 _springGoalPositionBlock, float _springUpTime, float _springDownTime)
    {
        rigidData.springHeigtBlock = _springHeigtBlock;
        rigidData.springGoalPositionBlock = _springGoalPositionBlock;
        rigidData.springUpTime = _springUpTime;
        rigidData.springDownTime = _springDownTime;
        rigidData.isSpring = true;
    }

    public void ChangeBodyColor(Color color)
    {
        spriteRenderer.color = color;
    }
}