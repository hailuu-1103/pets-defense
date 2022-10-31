using UnityEngine;

namespace Bullets
{
    using Enemy;

    /// <summary>
    /// Arrow fly trajectory.
    /// </summary>
    public class BulletArrow : MonoBehaviour, IBullet
    {
        // Damage amount
        public float damage = 1;

        // Maximum life time
        public float lifeTime = 3f;

        // Starting speed
        public float speed = 3f;

        // Constant acceleration
        public float speedUpOverTime = 0.5f;

        // If target is close than this distance - it will be hitted
        public float hitDistance = 0.2f;

        // Ballistic trajectory offset (in distance to target)
        public float ballisticOffset = 0.5f;

        // Do not rotate bullet during fly
        public bool freezeRotation = false;


        // From this position bullet was fired
        private Vector2 originPoint;

        // Aimed target
        private Transform target;

        // Last target's position
        private Vector2 aimPoint;

        // Current position without ballistic offset
        private Vector2 myVirtualPosition;

        // Position on last frame
        private Vector2 myPreviousPosition;

        // Counter for acceleration calculation
        private float counter;

        // Image of this bullet
        private SpriteRenderer sprite;

        [SerializeField] private string type;


        /// <summary>
        /// Set damage amount for this bullet.
        /// </summary>
        /// <param name="damage">Damage.</param>
        public void SetDamage(float damage) { this.damage = damage; }

        /// <summary>
        /// Fire bullet towards specified target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void Fire(Transform target)
        {
            this.sprite = this.GetComponent<SpriteRenderer>();
            // Disable sprite on first frame beqause we do not know fly direction yet
            this.sprite.enabled = false;
            this.originPoint    = this.myVirtualPosition = this.myPreviousPosition = this.transform.position;
            this.target         = target;
            this.aimPoint       = target.position;
            // Destroy bullet after lifetime
            Destroy(this.gameObject, this.lifeTime);
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        private void Update()
        {
            this.counter += Time.deltaTime;
            // Add acceleration
            this.speed += Time.deltaTime * this.speedUpOverTime;
            if (this.target != null)
            {
                this.aimPoint = this.target.position;
            }

            // Calculate distance from firepoint to aim
            Vector2 originDistance = this.aimPoint - this.originPoint;
            // Calculate remaining distance
            Vector2 distanceToAim = this.aimPoint - (Vector2)this.myVirtualPosition;
            // Move towards aim
            this.myVirtualPosition = Vector2.Lerp(this.originPoint, this.aimPoint, this.counter * this.speed / originDistance.magnitude);
            // Add ballistic offset to trajectory
            this.transform.position = this.AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
            // Rotate bullet towards trajectory
            this.LookAtDirection2D((Vector2)this.transform.position - this.myPreviousPosition);
            this.myPreviousPosition = this.transform.position;
            this.sprite.enabled     = true;
            // Close enough to hit
            if (distanceToAim.magnitude <= this.hitDistance)
            {
                if (this.target != null)
                {
                    // If target can receive damage
                    Enemy enemy = this.target.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        //damageTaker.TakeDamage(damage);

                        if (this.type.Contains("burn"))
                        {
                            enemy.BurnDamage(3, 1);
                        }
                        else if (this.type.Contains("slow"))
                        {
                            enemy.TakeDamage(1, "slow");
                        }
                    }
                }

                // Destroy bullet
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Looks at direction2d.
        /// </summary>
        /// <param name="direction">Direction.</param>
        private void LookAtDirection2D(Vector2 direction)
        {
            if (this.freezeRotation == false)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// Adds ballistic offset to trajectory.
        /// </summary>
        /// <returns>The ballistic offset.</returns>
        /// <param name="originDistance">Origin distance.</param>
        /// <param name="distanceToAim">Distance to aim.</param>
        private Vector2 AddBallisticOffset(float originDistance, float distanceToAim)
        {
            if (this.ballisticOffset > 0f)
            {
                // Calculate sinus offset
                float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
                offset *= originDistance;
                // Add offset to trajectory
                return (Vector2)this.myVirtualPosition + (this.ballisticOffset * offset * Vector2.up);
            }
            else
            {
                return this.myVirtualPosition;
            }
        }
    }
}