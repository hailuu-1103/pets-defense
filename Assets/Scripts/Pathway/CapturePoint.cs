using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// If enemy rise this point - player will be defeated.
/// </summary>
public class CapturePoint : MonoBehaviour
{
    // Enemy already reached capture point
    private bool alreadyCaptured;

    public static int Health = 10;

    public TextMeshProUGUI healthTxt;

    /// <summary>
    /// Raises the trigger enter2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    /// 

    private void Start()
    {
        healthTxt.text = Health.ToString();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If collision allowed for this scene
        if (LevelManager.IsCollisionValid(gameObject.tag, other.gameObject.tag) == true)
        {
            if (alreadyCaptured == false)
            {
                if (Health < 0)
                {
                    alreadyCaptured = true;
                    EventManager.TriggerEvent("Captured", other.gameObject, null);
                }

                Destroy(other.gameObject);

                Health--;
                healthTxt.text = Health.ToString();

            }
        }
    }
}

