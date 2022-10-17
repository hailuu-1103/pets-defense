using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// User interface and events manager.
/// </summary>
public class UiManager : MonoBehaviour
{
    // Next level scene name
    public string nextLevel;
    // Pause menu canvas
    public GameObject pauseMenu;
    // Defeat menu canvas
    public GameObject defeatMenu;
    // Victory menu canvas
    public GameObject victoryMenu;
    // Level interface
    public GameObject levelUI;
    // Avaliable gold amount
    public Text goldAmount;
    // Add sound effect
    private AudioSource audioSource;
    // Is game paused?
    private bool paused;

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
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        Debug.Assert(pauseMenu && defeatMenu && victoryMenu && levelUI && goldAmount, "Wrong initial parameters");
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.Play();
        GoToLevel();
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {

        if (paused == false)
        {
            // Pause on escape button
            if (Input.GetButtonDown("Cancel") == true)
            {
                PauseGame(true);
                GoToPauseMenu();
            }
            // User press mouse button
            if (Input.GetMouseButtonDown(0) == true)
            {
                // Check if pointer over UI components
                GameObject hittedObj = null;
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                foreach (RaycastResult res in results)
                {
                    if (res.gameObject.CompareTag("ActionIcon"))
                    {
                        hittedObj = res.gameObject;
                        break;
                    }
                }
                if (results.Count <= 0) // No UI components on pointer
                {
                    // Check if pointer over colliders
                    RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
                    foreach (RaycastHit2D hit in hits)
                    {
                        // If this is tower collider
                        if (hit.collider.gameObject.CompareTag("Tower"))
                        {
                            hittedObj = hit.collider.gameObject;
                            break;
                        }
                    }
                }
                // Send message with user click data
                EventManager.TriggerEvent("UserClick", hittedObj, null);
            }
        }
    }

    /// <summary>
    /// Stop current scene and load new scene
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    private void LoadScene(string sceneName)
    {
        EventManager.TriggerEvent("SceneQuit", null, null);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Start new game.
    /// </summary>
    public void NewGame()
    {
        GoToLevel();
        PauseGame(false);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        GoToLevel();
        PauseGame(false);
    }

    /// <summary>
    /// Gos to main menu.
    /// </summary>
    public void GoToMainMenu()
    {
        LoadScene("MainMenu");
    }

    /// <summary>
    /// Closes all UI canvases.
    /// </summary>
    private void CloseAllUI()
    {
        pauseMenu.SetActive(false);
        defeatMenu.SetActive(false);
        victoryMenu.SetActive(false);
        levelUI.SetActive(false);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    /// <param name="pause">If set to <c>true</c> pause.</param>
    private void PauseGame(bool pause)
    {
        paused = pause;
        // Stop the time on pause
        Time.timeScale = pause ? 0f : 1f;
        EventManager.TriggerEvent("GamePaused", null, pause.ToString());
    }

    /// <summary>
    /// Gos to pause menu.
    /// </summary>
    private void GoToPauseMenu()
    {
        PauseGame(true);
        CloseAllUI();
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Gos to level.
    /// </summary>
    private void GoToLevel()
    {
        CloseAllUI();
        levelUI.SetActive(true);
        PauseGame(false);
    }

    /// <summary>
    /// Gos to defeat menu.
    /// </summary>
    public void GoToDefeatMenu()
    {
        PauseGame(true);
        CloseAllUI();
        defeatMenu.SetActive(true);
    }

    /// <summary>
    /// Gos to victory menu.
    /// </summary>
    public void GoToVictoryMenu()
    {
        PauseGame(true);
        CloseAllUI();
        victoryMenu.SetActive(true);
    }

    /// <summary>
    /// Start next level.
    /// </summary>
    public void GoToNextLevel()
    {
        LoadScene(nextLevel);
    }

    /// <summary>
    /// Restarts current level.
    /// </summary>
    public void RestartLevel()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        LoadScene(activeScene);
    }

    /// <summary>
    /// Gets current gold amount.
    /// </summary>
    /// <returns>The gold.</returns>
    private int GetGold()
    {
        int gold;
        int.TryParse(goldAmount.text, out gold);
        return gold;
    }

    /// <summary>
    /// Sets gold amount.
    /// </summary>
    /// <param name="gold">Gold.</param>
    private void SetGold(int gold)
    {
        goldAmount.text = gold.ToString();
    }

    /// <summary>
    /// Adds the gold.
    /// </summary>
    /// <param name="gold">Gold.</param>
    private void AddGold(int gold)
    {
        SetGold(GetGold() + gold);
    }

    /// <summary>
    /// Spends the gold if it is.
    /// </summary>
    /// <returns><c>true</c>, if gold was spent, <c>false</c> otherwise.</returns>
    /// <param name="cost">Cost.</param>
    public bool SpendGold(int cost)
    {
        bool res = false;
        int currentGold = GetGold();
        if (currentGold >= cost)
        {
            SetGold(currentGold - cost);
            res = true;
        }
        return res;
    }

    /// <summary>
    /// On unit die.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UnitDie(GameObject obj, string param)
    {
        // If this is enemy
        if (obj.CompareTag("Enemy"))
        {
            Price price = obj.GetComponent<Price>();
            if (price != null)
            {
                // Add gold for enemy kill
                AddGold(price.price);
            }
        }
    }
}
