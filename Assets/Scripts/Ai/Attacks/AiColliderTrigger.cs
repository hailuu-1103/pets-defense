using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dynamic filter for AI collision mask
/// </summary>
public class AiColliderTrigger : MonoBehaviour
{
    // Allowed objects tags for collision detection
    public List<string> tags = new();

    // My collider
    private Collider2D col;
    // AI behaviour component in parent object
    private AiBehavior aiBehavior;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        this.col        = this.GetComponent<Collider2D>();
        this.aiBehavior = this.GetComponentInParent<AiBehavior>();
        Debug.Assert(this.col && this.aiBehavior, "Wrong initial parameters");
    }

    /// <summary>
    /// Determines whether this instance is tag allowed the specified tag.
    /// </summary>
    /// <returns><c>true</c> if this instance is tag allowed the specified tag; otherwise, <c>false</c>.</returns>
    /// <param name="tag">Tag.</param>
    private bool IsTagAllowed(string tag)
    {
        bool res = false;
        if (this.tags.Count > 0)
        {
            foreach (string str in this.tags)
            {
                if (str == tag)
                {
                    res = true;
                    break;
                }
            }
        }
        else
        {
            res = true;
        }
        return res;
    }

    /// <summary>
    /// Raises the trigger enter2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (this.IsTagAllowed(other.tag) == true)
        {
            // Notify AI behavior about this event
            this.aiBehavior.TriggerEnter2D(this.col, other);
        }
    }

    /// <summary>
    /// Raises the trigger stay2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (this.IsTagAllowed(other.tag))
        {
            // Notify AI behavior about this event
            this.aiBehavior.TriggerStay2D(this.col, other);
        }
    }

    /// <summary>
    /// Raises the trigger exit2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        if (this.IsTagAllowed(other.tag) == true)
        {
            // Notify AI behavior about this event
            this.aiBehavior.TriggerExit2D(this.col, other);
        }
    }
}
