using System.Collections;
using System.Collections.Generic;
using Manage;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

/// <summary>
/// User interface and events manager.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private Button saveBtn;

    #region Zenject

    private SaveLoadSystem saveLoadSystem;
    private GameState      gameState;

    #endregion
    
    // Next level scene name
    public string nextLevel;
    // Pause menu canvas
    public GameObject pauseMenu;
    // Defeat menu canvas
    public GameObject defeatMenu;
   
    // Level interface
    public GameObject levelUI;
    // Available gold amount
    public Text goldAmount;

    [Inject]
    private void Construct(SaveLoadSystem system, GameState state)
    {
        this.saveLoadSystem = system;
        this.gameState      = state;
    }
    // Is game paused?
    private bool paused;

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    private void OnEnable()
    {
        EventManager.StartListening("UnitDie", this.UnitDie);
        this.saveBtn.onClick.AddListener(this.OnClickSaveButton);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    private void OnDisable()
    {
        EventManager.StopListening("UnitDie", this.UnitDie);
        this.saveBtn.onClick.RemoveListener(this.OnClickSaveButton);
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        Debug.Assert(this.pauseMenu && this.defeatMenu && this.levelUI && this.goldAmount, "Wrong initial parameters");
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        this.SetGold();
        this.GoToLevel();
        Debug.Log(this.gameState);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    private void Update()
    {
        if (this.paused == false)
        {
            // Pause on escape button
            if (Input.GetButtonDown("Cancel") == true)
            {
                this.PauseGame(true);
                this.GoToPauseMenu();
            }
            // User press mouse button
            if (Input.GetMouseButtonDown(0) == true)
            {
                // Check if pointer over UI components
                GameObject hittedObj = null;
                var pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                foreach (var res in results)
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
                    var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
                    foreach (var hit in hits)
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
        this.GoToLevel();
        this.PauseGame(false);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        this.GoToLevel();
        this.PauseGame(false);
    }

    /// <summary>
    /// Gos to main menu.
    /// </summary>
    public void GoToMainMenu()
    {
        this.LoadScene("MainMenu");
    }

    /// <summary>
    /// Closes all UI canvases.
    /// </summary>
    private void CloseAllUI()
    {
        this.pauseMenu.SetActive(false);
        this.defeatMenu.SetActive(false);
        // this.levelUI.SetActive(false);
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    /// <param name="pause">If set to <c>true</c> pause.</param>
    private void PauseGame(bool pause)
    {
        this.paused = pause;
        // Stop the time on pause
        Time.timeScale = pause ? 0f : 1f;
        EventManager.TriggerEvent("GamePaused", null, pause.ToString());
    }

    /// <summary>
    /// Go to pause menu.
    /// </summary>
    private void GoToPauseMenu()
    {
        this.PauseGame(true);
        this.CloseAllUI();
        this.pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Go to level.
    /// </summary>
    private void GoToLevel()
    {
        this.CloseAllUI();
        this.levelUI.SetActive(true);
        this.PauseGame(false);
    }

    /// <summary>
    /// Go to defeat menu.
    /// </summary>
    public void GoToDefeatMenu()
    {
        this.PauseGame(true);
        this.CloseAllUI();
        this.defeatMenu.SetActive(true);
    }

    /// <summary>
    /// Go to victory menu.
    /// </summary>
    public void GoToVictoryMenu()
    {
        this.PauseGame(true);
        this.CloseAllUI();
    }

    /// <summary>
    /// Start next level.
    /// </summary>
    public void GoToNextLevel()
    {
        this.LoadScene(this.nextLevel);
    }

    /// <summary>
    /// Restart current level.
    /// </summary>
    public void RestartLevel()
    {
        var activeScene = SceneManager.GetActiveScene().name;
        this.LoadScene(activeScene);
    }

    /// <summary>
    /// Sets gold amount.
    /// </summary>
    private void SetGold()
    {
        this.goldAmount.text = this.gameState.goldCollected.ToString();
    }

    /// <summary>
    /// Adds the gold.
    /// </summary>
    /// <param name="gold">Gold.</param>
    private void AddGold(int gold)
    {
        this.gameState.goldCollected += gold;
    }

    /// <summary>
    /// Spends the gold if it is.
    /// </summary>
    /// <returns><c>true</c>, if gold was spent, <c>false</c> otherwise.</returns>
    /// <param name="cost">Cost.</param>
    public bool CanSpendGold(int cost)
    {
        var result      = false;
        var currentGold = this.gameState.goldCollected;
        if (currentGold >= cost)
        {
            this.gameState.goldCollected -= cost;
            this.SetGold();
            result = true;
        }
        return result;
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
            var price = obj.GetComponent<Price>();
            if (price != null)
            {
                // Add gold for enemy kill
                this.AddGold(price.price);
            }
        }
    }

    #region Callback

    private void OnClickSaveButton()
    {
        this.saveLoadSystem.SaveToFile();
    }

    #endregion
}
