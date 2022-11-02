using System.Collections;
using UnityEngine;

namespace Enemy
{
    
    /// <summary>
    /// This target can receive damage.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        AudioSource AudioSound;
        
        public AudioClip HitSound;
        public AudioClip DieSound;

        public float volume = 0.5f;


        // Start health
        [SerializeField] private float health;
        // Remaining health
        [SerializeField] private float currentHealth;
        // Hit visual effect duration
        [SerializeField] private float hitDisplayTime;

        // Image of this object
        private SpriteRenderer sprite;
        // Visualisation of hit
        private bool hitCoroutine;

    
        private HealthBar healthbar;

        /// <summary>
        /// Awake this instance.
        /// </summary>
        private void Awake()
        {
            AudioSound = GetComponent<AudioSource>();

            this.healthbar        = this.GetComponentInChildren<HealthBar>();
            this.currentHealth = this.health;
            this.healthbar.SetMaxHealth(this.health);
        

            this.sprite = this.GetComponentInChildren<SpriteRenderer>();
            Debug.Assert(this.sprite, "Wrong initial parameters");
        }


        /// <summary>
        /// Take damage.
        /// </summary>
        /// <param name="damage">Damage.</param>
        public void TakeDamage(float damage)
        {
            if (this.currentHealth > damage)
            {
                // Still alive
                this.currentHealth -= damage;

                AudioSound.PlayOneShot(HitSound, volume);

                this.healthbar.SetHealth(this.currentHealth);

                // If no coroutine now
                if (this.hitCoroutine == false)
                {
                    // Damage visualisation
                    this.StartCoroutine(this.DisplayDamage());
                }
            }
            else
            {
                // Die
                this.currentHealth = 0;
                this.Die();
            }
        }
    

        public void TakeDamage(float damage, string status)
        {
            if (status.Equals("slow"))
            {
                if (this.currentHealth > damage)
                {
                    // Still alive
                    this.currentHealth -= damage;

                    AudioSound.PlayOneShot(HitSound, volume);

                    this.healthbar.SetHealth(this.currentHealth);
                    this.healthbar.SetStatus("slow");
                    // If no coroutine now
                    if (this.hitCoroutine == false)
                    {
                        // Damage visualisation
                        this.StartCoroutine(this.DisplayDamage());
                    }
                }
                else
                {
                    // Die
                    this.currentHealth = 0;
                    this.Die();
                }
            }

        }


        public void BurnDamage(float time, float damage)
        {

            this.StartCoroutine(this.BurnDamage(damage));
        }
        /// <summary>
        /// Die this instance.
        /// </summary>
        public void Die()
        {
            EventManager.TriggerEvent("UnitDie", this.gameObject, null);
            AudioSound.PlayOneShot(DieSound, volume);
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Damage visualisation.
        /// </summary>
        /// <returns>The damage.</returns>
        private IEnumerator DisplayDamage()
        {
            this.hitCoroutine = true;
            Color originColor = this.sprite.color;
            float counter;
            // Set color to black and return to origin color over time
            for (counter = 0f; counter < this.hitDisplayTime; counter += Time.deltaTime)
            {
                this.sprite.color = Color.Lerp(originColor, Color.black, Mathf.PingPong(counter, this.hitDisplayTime));
                yield return new WaitForEndOfFrame();
            }
            this.sprite.color = originColor;
            this.hitCoroutine = false;
        }

        private IEnumerator BurnDamage(float damage)
        {
            float x = 3;
            while (x>0)
            {
                this.healthbar.SetStatus("burning");
                this.TakeDamage(damage);
                AudioSound.PlayOneShot(HitSound, volume);
                x--;
                yield return new WaitForSeconds(1);
            }
            this.healthbar.DisableStatus("burning");
        }
    }
}

