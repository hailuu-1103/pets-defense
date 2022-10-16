using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Enemy spawner.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Enemy wave structure.
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        // Delay before wave run
        public float timeBeforeWave;
        // List of enemies in this wave
        public List<GameObject> enemies;
    }

    // If enemy not set, this folder will use to get random enemy
    public string enemiesResourceFolder = "Prefabs/Enemies";
    // Enemies will have different speed in specified interval
    public float speedRandomizer = 0.2f;
    // Delay between enemies spawn in wave
    public float unitSpawnDelay = 0.8f;
    // Waves list for this spawner
    public List<Wave> waves;

    // Enemies will move along this pathway
    private Pathway path;
    // Nearest wave
    private Wave nextWave;
    // Delay counter
    private float counter;
    // Wave started
    private bool waveInProgress;
    // List for random enemy generation
    private List<GameObject> enemyPrefabs;
    // Buffer with active spawned enemies
    private List<GameObject> activeEnemies = new List<GameObject>();

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        path = GetComponentInParent<Pathway>();
        // Load enemies prefabs from specified directory
        enemyPrefabs = Resources.LoadAll<GameObject>(enemiesResourceFolder).ToList();
        Debug.Assert((path != null) && (enemyPrefabs != null), "Wrong initial parameters");
    }

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("UnitDie", UnitDie);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("UnitDie", UnitDie);
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        if (waves.Count > 0)
        {
            // Start from first wave
            nextWave = waves[0];
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        // Wait for next wave
        if ((nextWave != null) && (waveInProgress == false))
        {
            counter += Time.deltaTime;
            if (counter >= nextWave.timeBeforeWave)
            {
                counter = 0f;
                // Start new wave
                StartCoroutine(RunWave());
            }
        }
        // If all spawned enemies are dead
        if ((nextWave == null) && (activeEnemies.Count <= 0))
        {
            EventManager.TriggerEvent("AllEnemiesAreDead", null, null);
            // Turn off spawner
            enabled = false;
        }
    }

    /// <summary>
    /// Gets the next wave.
    /// </summary>
    private void GetNextWave()
    {
        int idx = waves.IndexOf(nextWave) + 1;
        if (idx < waves.Count)
        {
            nextWave = waves[idx];
        }
        else
        {
            nextWave = null;
        }
    }

    /// <summary>
    /// Runs the wave.
    /// </summary>
    /// <returns>The wave.</returns>
    private IEnumerator RunWave()
    {
        waveInProgress = true;
        foreach (GameObject enemy in nextWave.enemies)
        {
            GameObject prefab = null;
            prefab = enemy;
            // If enemy prefab not specified - get random enemy
            if (prefab == null)
            {
                prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            }
            // Create enemy
            GameObject newEnemy = Instantiate(prefab, transform.position, transform.rotation);
            // Set pathway
            newEnemy.GetComponent<AiStatePatrol>().path = path;
            NavAgent agent = newEnemy.GetComponent<NavAgent>();
            // Set speed offset
            agent.speed = Random.Range(agent.speed * (1f - speedRandomizer), agent.speed * (1f + speedRandomizer));
            // Add enemy to list
            activeEnemies.Add(newEnemy);
            // Wait for delay before next enemy run
            yield return new WaitForSeconds(unitSpawnDelay);
        }
        GetNextWave();
        waveInProgress = false;
    }

    /// <summary>
    /// On unit die.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UnitDie(GameObject obj, string param)
    {
        // If this is active enemy
        if (activeEnemies.Contains(obj) == true)
        {
            // Remove it from buffer
            activeEnemies.Remove(obj);
        }
    }
}
