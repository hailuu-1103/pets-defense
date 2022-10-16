using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows AI to operate move towards destination.
/// </summary>
public class AiStateMove : MonoBehaviour, IAiState
{
    // End point for moving
    public Transform destination;
    // Go to this state if agressive event occures
    public string agressiveAiState;
    // Go to this state if passive event occures
    public string passiveAiState;

    // Animation controller for this AI
    private Animation anim;
    // AI behavior of this object
    private AiBehavior aiBehavior;
    // Navigation agent of this gameobject
    NavAgent navAgent;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        this.aiBehavior = this.GetComponent<AiBehavior>();
        this.navAgent   = this.GetComponent<NavAgent>();
        this.anim       = this.GetComponentInParent<Animation>();
        Debug.Assert (this.aiBehavior && this.navAgent, "Wrong initial parameters");
    }

    /// <summary>
    /// Raises the state enter event.
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
    public void OnStateEnter (string previousState, string newState)
    {
        // Set destination for navigation agent
        this.navAgent.destination = this.destination.position;
        if (this.anim != null)
        {
            // Start moving
            this.navAgent.move = true;
            // Play animation
            this.anim.Play("Move");
        }
    }

    /// <summary>
    /// Raises the state exit event.
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
    public void OnStateExit (string previousState, string newState)
    {
        if (this.anim != null)
        {
            // Stop moving
            this.navAgent.move = false;
            // Stop animation
            this.anim.Stop();
        }
    }

    /// <summary>
    /// Fixed update for this instance.
    /// </summary>
    void FixedUpdate ()
    {
        // If destination reached
        if ((Vector2)this.transform.position == (Vector2)this.destination.position)
        {
            // Look at required direction
            this.navAgent.LookAt(this.destination.right);
            // Go to passive state
            this.aiBehavior.ChangeState(this.passiveAiState);
        }
    }

    /// <summary>
    /// Triggers the enter.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void TriggerEnter(Collider2D my, Collider2D other)
    {

    }

    /// <summary>
    /// Triggers the stay.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void TriggerStay(Collider2D my, Collider2D other)
    {

    }

    /// <summary>
    /// Triggers the exit.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void TriggerExit(Collider2D my, Collider2D other)
    {

    }
}
