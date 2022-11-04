using System;
using Cysharp.Threading.Tasks;
using GameData;
using GameFoundation.Scripts.AssetLibrary;
using Manage;
using Signals;
using UnityEngine;
using Zenject;

/// <summary>
/// Tower building and operation.
/// </summary>
public class Tower : MonoBehaviour
{
    private SignalBus      signalBus;
    private GameState      gameState;
    private DiContainer    diContainer;
    private IGameAssets    gameAssets;
    private SaveLoadSystem saveLoadSystem;

    // Prefab for building tree
    public GameObject buildingTreePrefab;

    // Attack range of this tower
    public GameObject range;

    // User interface manager
    private UIManager uiManager;

    // Level UI canvas for building tree display
    private Canvas canvas;

    // Collider of this tower
    private Collider2D bodyCollider;

    // Displayed building tree
    private BuildingTree activeBuildingTree;

    [Inject]
    private void Init(SignalBus signal, GameState state, IGameAssets assets, DiContainer container, SaveLoadSystem system)
    {
        this.signalBus      = signal;
        this.gameState      = state;
        this.gameAssets     = assets;
        this.diContainer    = container;
        this.saveLoadSystem = system;
    }
    private void OnDisable()
    {
        EventManager.StopListening("GamePaused", this.GamePaused);
        EventManager.StopListening("UserClick", this.UserClick);
    }
    private void Start()
    {
        EventManager.StartListening("GamePaused", this.GamePaused);
        EventManager.StartListening("UserClick", this.UserClick);
        this.signalBus.Subscribe<LoadGameSignal>(this.OnLoadGame);
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    private void Awake()
    {
        this.uiManager = FindObjectOfType<UIManager>();
        var canvases = FindObjectsOfType<Canvas>();
        foreach (var canv in canvases)
        {
            if (canv.CompareTag("LevelUI"))
            {
                this.canvas = canv;
                break;
            }
        }

        this.bodyCollider = this.GetComponent<Collider2D>();
        Debug.Assert(this.uiManager && this.canvas && this.bodyCollider, "Wrong initial parameters");
    }

    /// <summary>
    /// Opens the building tree.
    /// </summary>
    private void OpenBuildingTree()
    {
        if (this.buildingTreePrefab != null)
        {
            // Create building tree
            this.activeBuildingTree = Instantiate(this.buildingTreePrefab, this.canvas.transform).GetComponent<BuildingTree>();
            // Set it over the tower
            this.activeBuildingTree.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
            this.activeBuildingTree.myTower            = this;
            // Disable tower raycast
            this.bodyCollider.enabled = false;
        }
    }

    /// <summary>
    /// Closes the building tree.
    /// </summary>
    private void CloseBuildingTree()
    {
        if (this.activeBuildingTree != null)
        {
            Destroy(this.activeBuildingTree.gameObject);
            // Enable tower raycast
            this.bodyCollider.enabled = true;
        }
    }

    /// <summary>
    /// Builds the tower.
    /// </summary>
    /// <param name="towerPrefab">Tower prefab.</param>
    public void BuildTower(GameObject towerPrefab)
    {
        // Close active building tree
        this.CloseBuildingTree();
        var price = towerPrefab.GetComponent<Price>();
        // If enough gold
        if (this.uiManager.CanSpendGold(price.price))
        {
            // Create new tower and place it on same position
            var newTower = Instantiate(towerPrefab, this.transform.parent);
            newTower.transform.position = this.transform.position;
            newTower.transform.rotation = this.transform.rotation;
            this.gameState.towers.Add(new TowerData() { name = towerPrefab.name, horizontalPosition = newTower.transform.position.x, verticalPosition = newTower.transform.position.y });
            this.diContainer.InjectGameObject(newTower);
            // Destroy old tower
            Destroy(this.gameObject);
        }
    }

    private async void OnLoadGame()
    {
        this.saveLoadSystem.ReadFromFile("data");
        foreach (var towerData in this.saveLoadSystem.gameState.towers)
        {
            if (this.transform.position.x != towerData.horizontalPosition || this.transform.position.y != towerData.verticalPosition) continue;
            // Create new tower and place it on same position
            var newTower = Instantiate(await this.gameAssets.LoadAssetAsync<GameObject>(towerData.name), this.transform.parent);
            newTower.transform.position = this.transform.position;
            newTower.transform.rotation = this.transform.rotation;
            // Destroy old tower
            // Destroy(this.gameObject);
        }
    }

    public void SellTower(GameObject towerPrefab, int price)
    {
        // Close active building tree
        this.CloseBuildingTree();
        // If enough gold
        this.uiManager.AddGold(price);
        // Create new tower and place it on same position
        var newTower = Instantiate(towerPrefab, this.transform.parent);
        newTower.transform.position = this.transform.position;
        newTower.transform.rotation = this.transform.rotation;
        // Destroy old tower
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Disable tower raycast and close building tree on game pause.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void GamePaused(GameObject obj, string param)
    {
        if (param == bool.TrueString) // Paused
        {
            this.CloseBuildingTree();
            this.bodyCollider.enabled = false;
        }
        else // Unpaused
        {
            this.bodyCollider.enabled = true;
        }
    }

    /// <summary>
    /// On user click.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UserClick(GameObject obj, string param)
    {
        if (obj == this.gameObject) // This tower is clicked
        {
            // Show attack range
            this.ShowRange(true);
            if (this.activeBuildingTree == null)
            {
                // Open building tree if it is not
                this.OpenBuildingTree();
            }
        }
        else // Other click
        {
            // Hide attack range
            this.ShowRange(false);
            // Close active building tree
            this.CloseBuildingTree();
        }
    }

    /// <summary>
    /// Display tower's attack range.
    /// </summary>
    /// <param name="condition">If set to <c>true</c> condition.</param>
    private void ShowRange(bool condition)
    {
        if (this.range != null)
        {
            this.range.SetActive(condition);
        }
    }
}