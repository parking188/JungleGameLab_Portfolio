using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float speed;
    public float xOffset = 200.0f;
    public float yOffset = 0.0f;
    public float zOffset = -10.0f;

    [HideInInspector] public GameObject playerObject;

    void Update()
    {
        FollowObject();
    }

    public void InitCamera()
    {
        playerObject = GameManager.Instance.player.gameObject;
    }

    private void FollowObject()
    {
        if (playerObject)
        {
            Vector3 playerPosition = playerObject.transform.position;
            playerPosition.x += xOffset;
            playerPosition.y += yOffset;
            Vector3 lerpPosition = Vector3.Lerp(transform.position, playerPosition, speed * Time.deltaTime);
            lerpPosition.z = zOffset;
            transform.position = lerpPosition;
        }
    }
}
