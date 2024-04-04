using System;
using UnityEngine;

[Serializable]
public struct PlayerRigidbodyData
{
    // Inspector
    [Header("Features")]

    [Header("Physics")]
    public Vector2 gravity;

    [Header("Run")]
    [Tooltip("Block Per Second")]
    [Range(0f, 200f)]
    public float runSpeedPerBlock;
    [Range(0f, 2f)]
    public float runAccelerationTime;
    [Range(0f, 2f)]
    public float runDecelerationTime;

    [Header("Jump")]
    public float jumpHeightPerBlock;
    [Range(0.1f, 2f)]
    public float jumpUpTime;
    // [Tooltip("Time allow jump after off ground")]
    // public float jumpCoyoteTime;
    // [Tooltip("Time allow jump before on ground")]
    // public float jumpBufferTime;

    [Header("Air")]
    public float airTerminalVelocity;
    [Tooltip("Air Moving Speed. Multiply by Run Speed")]
    public float airMoving;
    // public float airControl;
    // public float airBrake;
    // state bools

    [Header("Boost")]
    [HideInInspector] public float boostVelocityPerBlock;
    [HideInInspector] public float boostTime;
    [HideInInspector] public float boostAccelerationTime;
    [HideInInspector] public float boostDecelerationTime;


    [Header("Dash")]
    public float dashDistanceBlock;
    public float dashTime;
    public float dashBufferTime;

    // Spring
    [HideInInspector] public Vector2 springGoalPositionBlock;
    [HideInInspector] public float springHeigtBlock;
    [HideInInspector] public float springUpTime;
    [HideInInspector] public float springDownTime;
    [HideInInspector] public bool isSpring;

    // landing Boost
    [Header("LandingBoost")]
    public float landingBoostVelocityPerBlock;
    public float landingBoostTime;
    public float landingBoostAccelerationTime;
    public float landingBoostDecelerationTime;
    [HideInInspector] public bool isLandingBoost;
    [HideInInspector] public bool canGetStar;

    // Velocities
    [HideInInspector] public Vector2 runVelocity;
    [HideInInspector] public Vector2 boostVelocity;
    [HideInInspector] public Vector2 jumpVelocity;
    [HideInInspector] public Vector2 dashVelocity;
    [HideInInspector] public Vector2 springVelocity;
    [HideInInspector] public Vector2 trampolineVelocity;
    [HideInInspector] public Vector2 tmpVelocity;

    // Valuse
    // physics
    [HideInInspector] public float blockSize;// = 8f;

    // run
    [HideInInspector] public float runSpeed;
    [HideInInspector] public float runDirection;
    public float GetRunDirection() { return runDirection; }
    [HideInInspector] public float runAcc;// = 0f;
    [HideInInspector] public float runDeAcc;// = 0f;

    // jump
    [HideInInspector] public float jumpStartVel;
    [HideInInspector] public float jumpUpAcc;
    [HideInInspector] public float jumpTimer;// = 0f;
    [HideInInspector] public bool desiredJump;// = false;
    [HideInInspector] public bool isJumping;// = false;
    [HideInInspector] public bool isAirJumping;// = false;
    [HideInInspector] public float jumpHeight;

    // dash
    [HideInInspector] public float dashDistance;
    [HideInInspector] public float dashVel;
    [HideInInspector] public float dashAcc;
    [HideInInspector] public float dashTimer;// = 0f;
    [HideInInspector] public float dashBufferTimer;// = 0f;
    [HideInInspector] public bool isDashBufferTime;// = 0f;
    [HideInInspector] public bool isDashing;// = false;
    [HideInInspector] public Vector2 dashDirection;// = new Vector2(1, 0);


    // boost
    [HideInInspector] public float boostDistance;
    [HideInInspector] public float boostTimer; // = 0f;
    [HideInInspector] public bool isBoosting;
    [HideInInspector] public float boostAcc;
    [HideInInspector] public float boostDeAcc;
    [HideInInspector] public float boostSpeed;
    [HideInInspector] public float boostDirection;

    
    [HideInInspector] public Vector2 springGoalPosition;
    [HideInInspector] public float springHeigt;
    [HideInInspector] public float springStartVelY;
    [HideInInspector] public float springUpAcc;
    [HideInInspector] public float springDownAcc;
    [HideInInspector] public float springUpVelX;
    [HideInInspector] public float springDownVelX;
    [HideInInspector] public float springTimer; // = 0f;

    
    // trampoline
    [HideInInspector] public Vector2 trampolineDirection;
    [HideInInspector] public float trampolineTime;
    [HideInInspector] public float trampolineDistance;
    
    [HideInInspector] public float trampolineStartVelocity;
    [HideInInspector] public Vector2 trampolineDirectionNorm;
    [HideInInspector] public  Vector2 trampolineDeAcc;
    [HideInInspector] public float trampolineTimer; // = 0f;
    [HideInInspector] public bool isTrampolining; // = false;

}

    
