using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameData;
using Installer;
using Manage;
using Signals;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utils;
using Zenject;

/// <summary>
/// User interface and events manager.
/// </summary>
public class UIManager : MonoBehaviour
{
    private                  AudioSource audioSource;
    [SerializeField] private Button      saveBtn;
    [SerializeField] private Button      loadBtn;
    [SerializeField] private Button      submitBtn;
    [SerializeField] private Button      settingBtn;
    [SerializeField] private Button      resumeBtn;
    [SerializeField] private Button      cancelBtn;
    [SerializeField] private Button      quitBtn;
    

    [SerializeField] private GameObject      readInputObj;
    [SerializeField] private GameObject      loadInputObj;
    [SerializeField] private GameObject      layoutBase;
    [SerializeField] private TMP_InputField  inputField;
    [SerializeField] private GameObject      listFileObj;
    [SerializeField] private GameObject      pauseMenu;
    [SerializeField] private GameObject      defeatMenu;
    [SerializeField] private GameObject      levelUI;
    [SerializeField] private TextMeshProUGUI goldTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;

    #region Zenject

    private SignalBus      signalBus;
    private SaveLoadSystem saveLoadSystem;
    private GameState      gameState;
    private SpawnPoint     spawnPoint;

    #endregion


    private List<TextMeshProUGUI> viewTexts = new();
    [Inject]
    private void Construct(SignalBus signal, SaveLoadSystem system, GameState state, SpawnPoint spawner)
    {
        this.saveLoadSystem = system;
        this.gameState      = state;
        this.signalBus      = signal;
        this.spawnPoint     = spawner;
    }

    // Is game paused?
    private bool paused;

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    private void OnEnable()
    {
        this.signalBus.Subscribe<NextWaveSignal>(this.SetUpWaveText);
        EventManager.StartListening("UnitDie", this.UnitDie);
        this.quitBtn.onClick.AddListener(this.OnClickQuitButton);
        this.cancelBtn.onClick.AddListener(this.OnClickCancelButton);
        this.resumeBtn.onClick.AddListener(this.OnClickResumeButton);
        this.settingBtn.onClick.AddListener(this.OnClickSettingBtn);
        this.submitBtn.onClick.AddListener(this.OnClickSubmitButton);
        this.saveBtn.onClick.AddListener(this.OnClickSaveButton);
        this.loadBtn.onClick.AddListener(this.OnClickLoadButton);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    private void OnDisable()
    {
        this.signalBus.TryUnsubscribe<NextWaveSignal>(this.SetUpWaveText);
        EventManager.StopListening("UnitDie", this.UnitDie);
        this.quitBtn.onClick.RemoveListener(this.OnClickQuitButton);
        this.cancelBtn.onClick.RemoveListener(this.OnClickCancelButton);
        this.resumeBtn.onClick.RemoveListener(this.OnClickResumeButton);
        this.settingBtn.onClick.RemoveListener(this.OnClickSettingBtn);
        this.submitBtn.onClick.RemoveListener(this.OnClickSubmitButton);
        this.saveBtn.onClick.RemoveListener(this.OnClickSaveButton);
        this.loadBtn.onClick.RemoveListener(this.OnClickLoadButton);
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake() { Debug.Assert(this.pauseMenu && this.defeatMenu && this.levelUI && this.goldTxt, "Wrong initial parameters"); }

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        this.readInputObj.SetActive(false);
        this.loadInputObj.SetActive(false);
        this.SetGold();
        this.GoToLevel();
        for (var i = 0; i < this.listFileObj.transform.childCount; i++)
        {
            var btn = this.listFileObj.transform.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(() => this.OnClickLoadFileButton(btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text));
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    private void Update()
    {
        if (this.paused == false)
        {
            // Pause on escape button
            if (Input.GetButtonDown("Cancel"))
            {
                this.PauseGame(true);
                this.GoToPauseMenu();
            }

            // User press mouse button
            if (Input.GetMouseButtonDown(0))
            {
                // Check if pointer over UI components
                GameObject hittedObj   = null;
                var        pointerData = new PointerEventData(EventSystem.current);
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
    private void SetUpWaveText()
    {
        // TODO Refactor CODE
        this.waveTxt.text = Mathf.CeilToInt(this.gameState.wave).ToString();
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
    public void GoToMainMenu() { this.LoadScene("MainMenu"); }

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
    private void SetGold() { this.goldTxt.text = this.gameState.goldCollected.ToString(); }

    /// <summary>
    /// Adds the gold.
    /// </summary>
    /// <param name="gold">Gold.</param>
    private void AddGold(int gold) { this.gameState.goldCollected += gold; }

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

    private void OnClickCancelButton()
    {
        this.readInputObj.SetActive(false);
        this.layoutBase.SetActive(true);
    }
    private void OnClickResumeButton()
    {
        this.PauseGame(false);
        this.levelUI.SetActive(true);
        this.pauseMenu.SetActive(false);

    }
    private void OnClickSettingBtn()
    {
        this.PauseGame(true);
        this.levelUI.SetActive(false);
        this.pauseMenu.SetActive(true);
    }
    private void OnClickSaveButton()
    {
        this.layoutBase.SetActive(false);
        this.readInputObj.SetActive(true);
    }
    private void OnClickSubmitButton()
    {
        // JsonUtil.Create("");
        if (!this.inputField.text.Equals(""))
        {
            this.saveLoadSystem.SaveToFile(this.inputField.text);
        }
    }

    private void OnClickLoadButton()
    {
        var folder    = new DirectoryInfo("Assets/StreamingAssets/TempData");
        var fileInfos = folder.GetFiles();
        for (var i = 0; i < 5; i++)
        {
            var file = fileInfos[i];
            this.listFileObj.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = file.Name.Contains(".meta") ? "" : file.Name.Replace(".json", "");
        }

        for (var i = 0; i < this.listFileObj.transform.childCount; i++)
        {
            var txt = this.listFileObj.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (txt.Trim().Equals(""))
            {
                this.listFileObj.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        this.loadInputObj.SetActive(true);
    }

    private void OnClickLoadFileButton(string path)
    {
        this.signalBus.Fire<LoadGameSignal>();
        this.spawnPoint.SpawnFromWave(path);
        this.pauseMenu.SetActive(false);
        this.levelUI.SetActive(true);
    }

    private void OnClickQuitButton()
    {
        Application.Quit();
    }
    #endregion
}