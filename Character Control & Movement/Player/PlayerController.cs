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
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CapsuleCollider playerCollider;
    [HideInInspector] public InputData input;

    [Header("Gravity")]
    public float gravity = 0.7f;
    public float gravityAcceleration = 1.0f;
    public bool isGrounded;
    private Vector3 groundNormal;

    [Header("Movement")]
    public float moveSpeed;
    public float moveAcceleration;
    public float jumpImpulse;
    public float dashSpeed;
    public float rotationSmoothTime = 0.12f;
    public float rotationVelocity;

    [Header("Friction")]
    [Range(0f, 10f)]
    public float xzFriction;
    [Range(0f, 10f)]
    public float airFriction;

    private Vector3 moveVelocity;
    private Vector3 gravityVelocity;
    private Vector3 frictionVelocity;
    private Vector3 finalVelocity;

    [HideInInspector] public Vector3 inputDirection;
    [HideInInspector] public Vector3 moveDirection;

    private Camera playerCamera;
    private int playerLayerMask;
    private int groundLayerMask;

    private Vector3 CapsuleTopCenterPoint => new Vector3(playerCollider.transform.position.x, playerCollider.transform.position.y + playerCollider.height - playerCollider.radius, playerCollider.transform.position.z);
    private Vector3 CapsuleBottomCenterPoint => new Vector3(playerCollider.transform.position.x, playerCollider.transform.position.y + playerCollider.radius, playerCollider.transform.position.z);
    private float CapsuleCastRadius => playerCollider.radius * 0.9f;

    public Vector3 XZVelocity => new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
    public Vector3 YVelocity => new Vector3(0f, rigid.velocity.y, 0f);

    void Awake()
    {
        player = GetComponent<Player>();
        player.SetController(this);
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponentInChildren<CapsuleCollider>();

        input = new InputData();
        playerCamera = Camera.main;
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        moveVelocity = Vector3.zero;
        gravityVelocity = Vector3.zero;
        finalVelocity = Vector3.zero;
    }

    private void Update()
    {
        PlayerInputCustom.Instance.GetInput(out input);
        CalcMoveDirection();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        FixedUpdatePlayerMove();
    }

    private void CheckGrounded()
    {
        float checkDistance = 0.1f;
        isGrounded = Physics.SphereCast(CapsuleBottomCenterPoint, CapsuleCastRadius, Vector3.down, out var hit, checkDistance, groundLayerMask, QueryTriggerInteraction.Ignore);

        if(isGrounded)
            groundNormal = hit.normal;

        animator.SetBool("Grounded", isGrounded);
    }

    private void FixedUpdatePlayerMove()
    {
        CalculateGravity();
        CalcFriction();
        finalVelocity = rigid.velocity + frictionVelocity + moveVelocity + gravityVelocity;
        rigid.velocity = finalVelocity;
    }

    private void CalculateGravity()
    {
        if (isGrounded == false
            || gravityVelocity.y > 0f)
        {
            Vector3 curYVelocity = YVelocity;
            if (curYVelocity.y > -gravity)
            {
                gravityVelocity = Vector3.up * -gravityAcceleration * Time.fixedDeltaTime;

                Vector3 nextGravityVelocity = curYVelocity + gravityVelocity;
                if (nextGravityVelocity.y < -gravity)
                {
                    gravityVelocity -= nextGravityVelocity - (nextGravityVelocity.normalized * -gravity);
                }
            }
            else
            {
                gravityVelocity = Vector3.zero;
            }
        }
        else
        {
            gravityVelocity = Vector3.zero;
        }
    }

    public void CalcMoveDirection()
    {
        inputDirection = new Vector3(input.direction.x, 0.0f, input.direction.z).normalized;
        float inputMagnitude = inputDirection.magnitude;
        float animSpeedInterpolation = 15f;

        if (inputMagnitude > 0.1f)
        {
            animator.SetFloat("MoveSpeed", Mathf.Lerp(animator.GetFloat("MoveSpeed"), 1f, Time.deltaTime * animSpeedInterpolation));
            Vector3 forwardDirection = playerCamera.transform.forward;
            Vector3 rightDirection = playerCamera.transform.right;
            Vector3 rotateDirection = (forwardDirection * input.direction.z + rightDirection * input.direction.x).normalized;
            rotateDirection.y = 0f;
            moveDirection = rotateDirection;
        }
        else
        {
            animator.SetFloat("MoveSpeed", Mathf.Lerp(animator.GetFloat("MoveSpeed"), 0f, Time.deltaTime * animSpeedInterpolation));
            moveDirection = Vector3.zero;
        }
    }

    public void MoveFixedUpdate()
    {
        if (inputDirection.magnitude > 0.1f)
        {
            Vector3 curMoveVelocity = XZVelocity;
            if (curMoveVelocity.magnitude < moveSpeed)
            {
                moveVelocity = moveDirection * moveAcceleration * Time.fixedDeltaTime;

                Vector3 nextMoveVelocity = curMoveVelocity + moveVelocity;
                if(nextMoveVelocity.magnitude > moveSpeed)
                {
                    moveVelocity -= nextMoveVelocity - (nextMoveVelocity.normalized * moveSpeed);
                }
            }
            else
            {
                moveVelocity = Vector3.zero;
            }
        }
    }

    public void StopWhenLowSpeed()
    {
        float stopSpeed = 0.5f;
        if(XZVelocity.magnitude < stopSpeed)
        {
            ResetRigidXZVelocity();
        }
    }

    public void ResetMoveVelocity()
    {
        moveVelocity = Vector3.zero;
    }

    public void ResetRigidXZVelocity()
    {
        rigid.velocity = new Vector3(0f, rigid.velocity.y, 0f);
    }

    public void RotateFixedUpdate()
    {
        if (inputDirection.magnitude > 0.1f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            playerCollider.transform.localRotation = Quaternion.Lerp(playerCollider.transform.localRotation, moveRotation, rotationVelocity * Time.fixedDeltaTime);
        }
    }

    public void Jump()
    {
        animator.SetBool("Jump", true);
        gravityVelocity = Vector3.up * jumpImpulse;
        rigid.velocity += gravityVelocity;
    }

    public void Dash()
    {
        animator.SetBool("Dash", true);
        rigid.velocity = Vector3.zero;
        rigid.velocity += Vector3.up * (jumpImpulse / 2f);
        rigid.velocity += moveDirection * dashSpeed;
    }

    public void ResetGravityVelocity()
    {
        gravityVelocity = Vector3.zero;
    }

    private void CalcFriction()
    {
        if(isGrounded)
        {
            frictionVelocity = -XZVelocity * xzFriction * Time.fixedDeltaTime;
        }
        else
        {
            frictionVelocity = -XZVelocity * airFriction * Time.fixedDeltaTime;
        }
    }
}