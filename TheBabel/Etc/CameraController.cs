using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Control")]
    public float fLerpSpeed = 2.0f;
    public float fZOffset = -10.0f;

    private PlayerController player;
    private new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if(player == null)
        {
            player = GameManager.Instance.player;
        }
        else
        {
            FollowObject(player.gameObject);
        }
    }

    public void FollowObject(GameObject obj)
    {
        Vector3 v3LerpPosition = Vector3.Lerp(transform.position, obj.transform.position, Time.deltaTime * fLerpSpeed);
        v3LerpPosition.z = fZOffset;
        transform.position = v3LerpPosition;
    }

    public IEnumerator UpsideDown()
    {
        float totalRotationX = 0.0f;
        float rotationSpeed = 60.0f;
        Time.timeScale = 0.0f;

        while(totalRotationX < 90.0f)
        {
            float rotationX = Time.unscaledDeltaTime * rotationSpeed;
            totalRotationX += rotationX;
            camera.transform.Rotate(Vector3.right * rotationX);
            yield return null;
        }

        fZOffset *= -1;
        GL.invertCulling = !GL.invertCulling;

        while(totalRotationX < 180.0f)
        {
            float rotationX = Time.unscaledDeltaTime * rotationSpeed;
            totalRotationX += rotationX;
            camera.transform.Rotate(Vector3.right * rotationX);
            yield return null;
        }

        if(GL.invertCulling)
        {
            camera.transform.eulerAngles = new Vector3(180.0f, 0.0f, 0.0f);
        }
        else
        {
            camera.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }

        player.gameObject.GetComponent<Rigidbody2D>().gravityScale *= -1.0f;
        player.fJumpPower *= -1.0f;
        Time.timeScale = 1.0f;
    }

    public IEnumerator cameraShake()
    {
        float shakePeriod = 0.1f;
        float shakeTime = 0.4f;
        float pastTime = 0.0f;
        float shakeIntensity = 0.6f;

        while(pastTime < shakeTime)
        {
            Vector3 cameraPosition = transform.position;
            Vector3 cameraShake = new Vector2(Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity));
            transform.position = cameraPosition + cameraShake;
            yield return new WaitForSeconds(shakePeriod);
            pastTime += shakePeriod;
        }
    }
}
