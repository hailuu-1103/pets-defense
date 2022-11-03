using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuildingSell : MonoBehaviour
{

    // Tower prefab for this icon
    public GameObject towerPrefab;
    

    // Parent building tree
    private BuildingTree myTree;

    [SerializeField]
    private int price;
    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("UserClick", UserClick);
    }
    

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("UserClick", UserClick);
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        // Get building tree from parent object
        myTree = transform.GetComponentInParent<BuildingTree>();

        //Debug.Assert(price && myTree, "Wrong initial parameters");
    }
    



    /// <summary>
    /// On user click.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UserClick(GameObject obj, string param)
    {
        // If clicked on this icon
        if (obj == gameObject)
        {
            // Build the tower
            myTree.Sell(towerPrefab,price);
        }

    }

}
