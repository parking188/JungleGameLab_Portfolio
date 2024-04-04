using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject objProjectilePrefab;
    public Vector3 v3ProjectileDirection;
    public float fSpawnStart = 1.0f;
    public float fSpawnDelay = 1.5f;
    public float fSpeed = 10.0f;
    public float fRange = 80.0f;

    void Start()
    {
        InvokeRepeating("SpawnProjectile", fSpawnStart, fSpawnDelay);
    }

    private void SpawnProjectile()
    {
        if(!GameManager.Instance.bIsGameActive)
        {
            return;
        }

        GameObject objProjectile = Instantiate(objProjectilePrefab, transform.position, objProjectilePrefab.transform.rotation);
        IEnemyProjectile projectileMove = objProjectile.GetComponent<IEnemyProjectile>();
        projectileMove.SetMoveDir(v3ProjectileDirection.normalized);
        projectileMove.speed = this.fSpeed;
        projectileMove.range = this.fRange;
    }
}
