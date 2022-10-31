using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Manage;
using Signals;
using UnityEngine;
using Utils;
using Zenject;
using Random = UnityEngine.Random;

/// <summary>
/// Enemy spawner.
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Enemy wave structure.
    /// </summary>
    [Serializable]
    public class Wave
    {
        // Delay before wave run
        public float timeBeforeWave;
        // List of enemies in this wave
        public List<GameObject> enemies;
    }

    [SerializeField] private Transform enemyHolder;

    #region Zenject

    private SignalBus signalBus;
    private GameState gameState;

    #endregion
   
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
    [SerializeField] private List<GameObject> enemyPrefabs = new();
    // Buffer with active spawned enemies
    private          List<GameObject> activeEnemies = new();
    private readonly string           Path          = "Assets/StreamingAssets/TempData/Temp.json";

    [Inject]
    private void Init(GameState state, SignalBus signal)
    {
        this.gameState = state;
        this.signalBus = signal;
    }
    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake ()
    {
        this.path = this.GetComponentInParent<Pathway>();
        
        Debug.Assert((this.path != null) && (this.enemyPrefabs != null), "Wrong initial parameters");
    }

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    private void OnEnable()
    {
        EventManager.StartListening("UnitDie", this.UnitDie);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    private void OnDisable()
    {
        EventManager.StopListening("UnitDie", this.UnitDie);
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        if (this.waves.Count > 0)
        {
            // Start from first wave
            this.nextWave = this.waves[0];
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    private void Update()
    {
        // Wait for next wave
        if ((this.nextWave != null) && (this.waveInProgress == false))
        {
            this.counter += Time.deltaTime;
            if (this.counter >= this.nextWave.timeBeforeWave)
            {
                this.counter = 0f;

                // Start new wave
                this.StartCoroutine(this.RunWave());
            }
        }
        // If all spawned enemies are dead
        if ((this.nextWave == null) && (this.activeEnemies.Count <= 0))
        {
            EventManager.TriggerEvent("AllEnemiesAreDead", null, null);
            // Turn off spawner
            this.enabled = false;
        }
    }

    /// <summary>
    /// Gets the next wave.
    /// </summary>
    private void GetNextWave()
    {
        var idx = this.waves.IndexOf(this.nextWave) + 1;
        if (idx < this.waves.Count)
        {
            this.nextWave = this.waves[idx];
        }
        else
        {
            this.nextWave = null;
        }
    }

    /// <summary>
    /// Runs the wave.
    /// </summary>
    /// <returns>The wave.</returns>
    private IEnumerator RunWave()
    {
        this.waveInProgress = true;
        foreach (var enemy in this.nextWave.enemies)
        {
            GameObject prefab = null;
            prefab = enemy;
            // If enemy prefab not specified - get random enemy
            if (prefab == null)
            {
                prefab = this.enemyPrefabs[Random.Range(0, this.enemyPrefabs.Count)];
            }
            // Create enemy
            var newEnemy = Instantiate(prefab, this.transform.position, this.transform.rotation, this.enemyHolder);
            // Set pathway
            newEnemy.GetComponent<AiStatePatrol>().path = this.path;
            // Add enemy to list
            this.activeEnemies.Add(newEnemy);
            // Wait for delay before next enemy run
            yield return new WaitForSeconds(this.unitSpawnDelay);
        }
        
        this.gameState.wave += 0.5f;
        this.signalBus.Fire<NextWaveSignal>();
        this.GetNextWave();
        this.waveInProgress = false;
    }

    public void SpawnInLoad()
    {
        this.gameState = JsonUtil.Load<GameState>(this.Path);
        foreach (var enemyData in this.gameState.enemies)
        {
            
        }
    }

    /// <summary>
    /// On unit die.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UnitDie(GameObject obj, string param)
    {
        // If this is active enemy
        if (this.activeEnemies.Contains(obj))
        {
            // Remove it from buffer
            this.activeEnemies.Remove(obj);
        }
    }
}
