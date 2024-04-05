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
    // cinemachine
    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    private const float _threshold = 0.01f;
    private float cameraSensitivity = 1.0f;

    [HideInInspector] public Player player;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public GravityControl gravityControl;
    public InputData input;

    [Header("Movement")]
    public float spaceWalkSpeed;
    public float spaceWalkDashSpeed;
    public float spaceWalkStopSpeed;
    public float moveSpeed;
    public float jumpSpeed;
    public Vector3 groundNormal;
    public bool isGround;
    
    void Awake()
    {
        player = GetComponent<Player>();
        player.SetController(this);
        rigid = GetComponent<Rigidbody>();
        gravityControl = GetComponent<GravityControl>();
        playerCamera = Camera.main;
        input = new InputData();
    }

    private void Update()
    {
        PlayerInputCustom.Instance.GetInput(out input);
        CameraRotation();
        CheckIsGround();
    }

    private void CheckIsGround()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(rigid.transform.position, -transform.up, out hit, 10))
        {
            groundNormal = hit.normal;
            if (hit.distance < 5.0f)
            {
                isGround = true;
            }
            else
            {
                isGround = false;
            }
        }
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;
            _cinemachineTargetYaw   += input.look.x * deltaTimeMultiplier * cameraSensitivity;
            _cinemachineTargetPitch += input.look.y * deltaTimeMultiplier * cameraSensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw   = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle >  360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}