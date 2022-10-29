using System.Collections.Generic;
using Ai.States;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Main script to operate all AI states
    /// </summary>
    public class AiBehavior : MonoBehaviour
    {
        // This state will be activate on start
        public string defaultState;

        // List with all states for this AI
        private readonly List<IAiState> aiStates = new();

        // The state that was before
        private IAiState previousState;

        // Active state
        private IAiState currentState;

        /// <summary>
        /// Start this instance.
        /// </summary>
        private void Start()
        {
            // Get all AI states from this gameobject
            var states = this.GetComponents<IAiState>();
            if (states.Length > 0)
            {
                foreach (var state in states)
                {
                    // Add state to list
                    this.aiStates.Add(state);
                }

                if (this.defaultState != null)
                {
                    // Set active and previous states as default state
                    this.previousState = this.currentState = this.GetComponent(this.defaultState) as IAiState;
                    if (this.currentState != null)
                    {
                        // Go to active state
                        this.ChangeState(this.currentState.GetType().ToString());
                    }
                    else
                    {
                        Debug.LogError("Incorrect default AI state " + this.defaultState);
                    }
                }
                else
                {
                    Debug.LogError("AI have no default state");
                }
            }
            else
            {
                Debug.LogError("No AI states found");
            }
        }

        /// <summary>
        /// Set AI to defalt state.
        /// </summary>
        public void GoToDefaultState()
        {
            this.previousState = this.currentState;
            this.currentState  = this.GetComponent(this.defaultState) as IAiState;
            this.NotifyOnStateExit();
            this.DisableAllStates();
            this.EnableNewState();
            this.NotifyOnStateEnter();
        }

        /// <summary>
        /// Change Ai state.
        /// </summary>
        /// <param name="state">State.</param>
        public void ChangeState(string state)
        {
            if (state != "")
            {
                // Try to find such state in list
                foreach (var aiState in this.aiStates)
                {
                    if (state == aiState.GetType().ToString())
                    {
                        this.previousState = this.currentState;
                        this.currentState  = aiState;
                        this.NotifyOnStateExit();
                        this.DisableAllStates();
                        this.EnableNewState();
                        this.NotifyOnStateEnter();
                        return;
                    }
                }

                Debug.Log("No such state " + state);
                // If have no such state - go to default state
                this.GoToDefaultState();
                Debug.Log("Go to default state " + this.aiStates[0].GetType().ToString());
            }
        }

        /// <summary>
        /// Turn off all AI states components.
        /// </summary>
        private void DisableAllStates()
        {
            foreach (var aiState in this.aiStates)
            {
                var state = this.GetComponent(aiState.GetType().ToString()) as MonoBehaviour;
                state.enabled = false;
            }
        }

        /// <summary>
        /// Turn on active AI state component.
        /// </summary>
        private void EnableNewState()
        {
            var state = this.GetComponent(this.currentState.GetType().ToString()) as MonoBehaviour;
            state.enabled = true;
        }

        /// <summary>
        /// Send OnStateExit notification to previous state.
        /// </summary>
        private void NotifyOnStateExit()
        {
            var prev   = this.previousState.GetType().ToString();
            var active = this.currentState.GetType().ToString();
            var state  = this.GetComponent(prev) as IAiState;
            state.OnStateExit(prev, active);
        }

        /// <summary>
        /// Send OnStateEnter notification to new state.
        /// </summary>
        private void NotifyOnStateEnter()
        {
            var prev   = this.previousState.GetType().ToString();
            var active = this.currentState.GetType().ToString();
            var state  = this.GetComponent(active) as IAiState;
            state.OnStateEnter(prev, active);
        }

        /// <summary>
        /// Triggers the enter2d.
        /// </summary>
        /// <param name="my">My.</param>
        /// <param name="other">Other.</param>
        public void TriggerEnter2D(Collider2D my, Collider2D other)
        {
            if (LevelManager.IsCollisionValid(this.gameObject.tag, other.gameObject.tag))
            {
                this.currentState.TriggerEnter(my, other);
            }
        }

        /// <summary>
        /// Triggers the stay2d.
        /// </summary>
        /// <param name="my">My.</param>
        /// <param name="other">Other.</param>
        public void TriggerStay2D(Collider2D my, Collider2D other)
        {
            if (LevelManager.IsCollisionValid(this.gameObject.tag, other.gameObject.tag) == true)
            {
                this.currentState.TriggerStay(my, other);
            }
        }

        /// <summary>
        /// Triggers the exit2d.
        /// </summary>
        /// <param name="my">My.</param>
        /// <param name="other">Other.</param>
        public void TriggerExit2D(Collider2D my, Collider2D other)
        {
            if (LevelManager.IsCollisionValid(this.gameObject.tag, other.gameObject.tag) == true)
            {
                this.currentState.TriggerExit(my, other);
            }
        }
    }
}