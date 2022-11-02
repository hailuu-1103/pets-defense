using System;
using System.Collections;
using System.Collections.Generic;
using Ai;
using Ai.States;
using Signals;
using UnityEngine;
using Zenject;

/// <summary>
/// Allows AI to move with specified path.
/// </summary>
public class AiStatePatrol : MonoBehaviour, IAiState
{
    private SignalBus signalBus;
    private bool      isPaused;
    // Specified path
    public Pathway path;
    // Need to loop path after last point is reached?
    public bool loop = false;
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
    // Current destination
    private Waypoint destination;

    [Inject]
    private void Init(SignalBus signal) { this.signalBus = signal;}
    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        this.aiBehavior = this.GetComponent<AiBehavior>();
        this.navAgent   = this.GetComponent<NavAgent>();
        this.anim       = this.GetComponent<Animation>();
        Debug.Assert (this.aiBehavior && this.navAgent, "Wrong initial parameters");
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {

    }
    /// <summary>
    /// Raises the state enter event.
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
    public void OnStateEnter (string previousState, string newState)
    {
        if (this.path == null)
        {
            // If I have no path - try to find it
            this.path = FindObjectOfType<Pathway>();
            Debug.Assert (this.path, "Have no path");
        }
        if (this.destination == null)
        {
            this.destination = this.path.GetNearestWaypoint (this.transform.position);
        }
        // Set destination for navigation agent
        this.navAgent.destination = this.destination.transform.position;
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
        if (this.destination != null)
        {
            // If destination reached
            if ((Vector2)this.destination.transform.position == (Vector2)this.transform.position)
            {
                // Get next waypoint from my path
                this.destination = this.path.GetNextWaypoint (this.destination, this.loop);
                if (this.destination != null)
                {
                    // Set destination for navigation agent
                    this.navAgent.destination = this.destination.transform.position;
                }
            }
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
        this.aiBehavior.ChangeState(this.agressiveAiState);
    }

    /// <summary>
    /// Triggers the exit.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void TriggerExit(Collider2D my, Collider2D other)
    {

    }

    /// <summary>
    /// Gets the remaining distance on pathway.
    /// </summary>
    /// <returns>The remaining path.</returns>
    public float GetRemainingPath()
    {
        Vector2 distance = this.destination.transform.position - this.transform.position;
        return (distance.magnitude + this.path.GetPathDistance(this.destination));
    }
}
