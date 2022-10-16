namespace Ai.Attacks
{
    using System;
    using UnityEngine;

    public class AttackMagic : MonoBehaviour, IAttack
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

        private void Start()
        {
            
        }
        
        private void Update()
        {
            if (this.cooldownCounter < this.cooldown)
            {
                this.cooldownCounter += Time.deltaTime;
            }
        }
        public void Attack(Transform target)
        {
            if (cooldownCounter >= cooldown)
            {
                cooldownCounter = 0f;
                Fire(target);
            }
        }
        /// <summary>
        /// Make ranged attack
        /// </summary>
        /// <param name="target">Target.</param>
        private void Fire(Transform target)
        {
            var speed = target.GetComponent<NavAgent>().speed;
            if (target != null)
            {
                speed                                 *= 0.8f;
                target.GetComponent<NavAgent>().speed =  speed;
                // Create arrow
                GameObject arrow  = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
                IBullet    bullet = arrow.GetComponent<IBullet>();
                bullet.SetDamage(damage);
                bullet.Fire(target);
                if (anim != null)
                {
                    anim.Play("AttackRanged");
                }
            }
        }
    }
}