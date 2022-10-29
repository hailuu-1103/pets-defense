using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all bullets.
/// </summary>
public interface IBullet
{
    void SetDamage(float damage);
    void Fire(Transform target);
}
