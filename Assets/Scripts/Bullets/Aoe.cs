using UnityEngine;

namespace Bullets
{
    using Enemy;

    /// <summary>
    /// Area Of Effect damage on destroing.
    /// </summary>
    public class Aoe : MonoBehaviour
    {
        // Area radius
        public float radius = 0.3f;

        // Damage amount
        public int damage = 3;

        // Explosion prefab
        public GameObject explosion;

        // Explosion visual duration
        public float explosionDuration = 1f;

        // Scene is closed now. Forbidden to create new objects on destroy
        private bool isQuitting;

        /// <summary>
        /// Raises the enable event.
        /// </summary>
        private void OnEnable() { EventManager.StartListening("SceneQuit", this.SceneQuit); }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        private void OnDisable() { EventManager.StopListening("SceneQuit", this.SceneQuit); }

        /// <summary>
        /// Raises the application quit event.
        /// </summary>
        private void OnApplicationQuit() { this.isQuitting = true; }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        private void OnDestroy()
        {
            // If scene is in progress
            if (this.isQuitting == false)
            {
                // Find all colliders in specified radius
                Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, this.radius);
                foreach (Collider2D col in cols)
                {
                    // If collision allowed by scene
                    if (LevelManager.IsCollisionValid(this.gameObject.tag, col.gameObject.tag) == true)
                    {
                        // If target can receive damage
                        Enemy enemy = col.gameObject.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(this.damage,"slow");
                        }
                    }
                }

                if (this.explosion != null)
                {
                    // Create explosion visual effect
                    Destroy(Instantiate<GameObject>(this.explosion, this.transform.position, this.transform.rotation), this.explosionDuration);
                }
            }
        }

        /// <summary>
        /// Raises on scene quit.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="param">Parameter.</param>
        private void SceneQuit(GameObject obj, string param) { this.isQuitting = true; }
    }
}