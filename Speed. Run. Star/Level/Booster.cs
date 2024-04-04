
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    //public PlayerController Controller;
    public float boostVelocityPerBlock;
    public float boostTime;
    public float boostAccelerationTime;
    public float boostDecelerationTime;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.SetBoostOn(boostVelocityPerBlock, boostTime, boostAccelerationTime, boostDecelerationTime);
        }
    }
}
