using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack with ranged weapon
/// </summary>
public class AttackRanged : MonoBehaviour, IAttack
{
    // Damage amount
    public int damage = 1;
    // Cooldown between attacks
    public float cooldown = 1f;
    // Prefab for arrows
    public GameObject arrowPrefab;
    // From this position arrows will fired
    public Transform firePoint;

    // Animation controller for this AI
    private Animation anim;
    // Counter for cooldown calculation
    private float cooldownCounter;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        this.anim            = this.GetComponentInParent<Animation>();
        this.cooldownCounter = this.cooldown;
        Debug.Assert(this.arrowPrefab && this.firePoint, "Wrong initial parameters");
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (this.cooldownCounter < this.cooldown)
        {
            this.cooldownCounter += Time.deltaTime;
        }
    }

    /// <summary>
    /// Attack the specified target if cooldown expired
    /// </summary>
    /// <param name="target">Target.</param>
    public void Attack(Transform target)
    {
        if (this.cooldownCounter >= this.cooldown)
        {
            this.cooldownCounter = 0f;
            this.Fire(target);
        }
    }

    /// <summary>
    /// Make ranged attack
    /// </summary>
    /// <param name="target">Target.</param>
    private void Fire(Transform target)
    {
        if (target != null)
        {
            // Create arrow
            var arrow  = Instantiate(this.arrowPrefab, this.firePoint.position, this.firePoint.rotation);
            var bullet = arrow.GetComponent<IBullet>();
            bullet.SetDamage(this.damage);
            bullet.Fire(target);
            if (this.anim != null)
            {
                this.anim.Play("AttackRanged");
            }
        }
    }
}
