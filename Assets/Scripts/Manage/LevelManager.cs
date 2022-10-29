using UnityEngine;

/// <summary>
/// Level control script.
/// </summary>
public class LevelManager : MonoBehaviour
{
    // User interface manager
    private UIManager uiManager;
    // Nymbers of enemy spawners in this level
    private int spawnNumbers;
    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        this.uiManager = FindObjectOfType<UIManager>();
        this.spawnNumbers = FindObjectsOfType<SpawnPoint>().Length;
        if (this.spawnNumbers <= 0)
        {
            Debug.LogError("Have no spawners");
        }
        Debug.Assert(this.uiManager, "Wrong initial parameters");
    }

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("Captured", this.Captured);
        EventManager.StartListening("AllEnemiesAreDead", this.AllEnemiesAreDead);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("Captured", this.Captured);
        EventManager.StopListening("AllEnemiesAreDead", this.AllEnemiesAreDead);
    }

    /// <summary>
    /// Determines if is collision valid for this scene.
    /// </summary>
    /// <returns><c>true</c> if is collision valid the specified myTag otherTag; otherwise, <c>false</c>.</returns>
    /// <param name="myTag">My tag.</param>
    /// <param name="otherTag">Other tag.</param>
    public static bool IsCollisionValid(string myTag, string otherTag)
    {
        var res = false;
        switch (myTag)
        {
            case "AirTower":
                switch (otherTag)
                {
                    case "Fly":
                        res = true;
                        break;
                }

                break;
            case "Tower":
            case "Defender":
                switch (otherTag)
                {
                    case "Enemy":
                        res = true;
                        break;
                }
                break;
            case "Enemy":
                switch (otherTag)
                {
                    case "Tower":
                        res = true;
                        break;
                }
                break;
            case "Bullet":
                switch (otherTag)
                {
                    case "Enemy" or "Fly":
                        res = true;
                        break;
                }
                break;
            case "CapturePoint":
                switch (otherTag)
                {
                    case "Enemy":
                        res = true;
                        break;
                }
                break;
            default:
                Debug.Log("Unknown collision tag => " + myTag + " - " + otherTag);
                break;
        }
        return res;
    }

    /// <summary>
    /// Enemy reached capture point.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void Captured(GameObject obj, string param)
    {
        // Defeat
        this.uiManager.GoToDefeatMenu();
    }

    /// <summary>
    /// All enemies are dead.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void AllEnemiesAreDead(GameObject obj, string param)
    {
        this.spawnNumbers--;
        // Enemies dead at all spawners
        if (this.spawnNumbers <= 0)
        {
            // Victory
            this.uiManager.GoToVictoryMenu();
        }
    }
}
