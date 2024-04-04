using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Behave();
    void Attack(IDamageable damageable);
}

public interface IEnemyProjectile
{
    float speed { get; set; }
    float range { get; set; }
    void SetMoveDir(Vector3 moveDirection);
}

public interface IDamageable
{
    void Damage(int damage);
    void Kill();
}

public interface IKnockBackable
{
    void KnockBack(Vector3 direction, float force);
}