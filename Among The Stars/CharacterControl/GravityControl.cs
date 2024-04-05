using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    [Header("Gravity")]
    public GravityOrbit Gravity;
    public float rotationSpeed = 10f;
    
    [Header("Camera & Cinemachine")]
    [SerializeField] private GameObject mainCamera;
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
    
    private Rigidbody rb;
    private PlayerController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController>();
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void Update()
    {
        SetPlanetGravity();
    }
    
    private void SetPlanetGravity()
    {
        if (Gravity)
        {
            Vector3 gravityUp = Vector3.zero;
            gravityUp = (transform.position - Gravity.transform.position).normalized;
            Vector3 localUp = transform.up;
            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * transform.rotation;
            rb.transform.rotation = targetRotation;
            transform.up = Vector3.Lerp(transform.up, gravityUp, rotationSpeed * Time.deltaTime);
            if (controller.isGround == false)
            {
                rb.AddForce(-gravityUp * Gravity.Gravity * rb.mass);
            }
        }
    }
}
