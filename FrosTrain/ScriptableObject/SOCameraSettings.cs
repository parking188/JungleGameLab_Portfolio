using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "ScriptableObjects/CameraSettings", order = 1)]
public class SOCameraSettings : ScriptableObject
{
    public float keyboardMovementMult = 5f;
    public float cameraKeyboardSpeed = 20f;
    public float cameraZoomSpeed = 1f;
    public List<float> cameraZoomSteps = new List<float>() { 15f, 20f, 30f };
    public List<float> cameraYOffsetSteps = new List<float>() { 0.5f, 1.85f, 6f };
    public List<float> exploreCameraZoomSteps = new List<float>() { 2.5f, 12.5f, 15f };
    public List<float> exploreCameraYOffsetSteps = new List<float>() { 0f, 0f, 0f };
    public float cameraZoffset = -10;
    public List<float> cameraSpeed = new List<float>() { 0.4f, 0.8f, 1.2f };
    public List<float> exploreCameraSpeed = new List<float>() { 0.7f, 1.2f, 1.5f };
    public float exploreCameraMinOffset = -10f;
    public float exploreCameraMaxOffset = 10f;

    public float trainToexploreTransitionSizeOffset = 30;
    public float exploreToTrainTransitionSizeOffset = 15;
}