using System.Collections;
using System.Collections.Generic;
using Ai.Attacks;
using UnityEngine;

/// <summary>
/// Allows AI to attack targets.
/// </summary>
public class AiStateAttack : MonoBehaviour, IAiState
{
    // Attack target closest to the capture point
    public bool useTargetPriority = false;
    // Go to this state if agressive event occures
    public string agressiveAiState;
    // Go to this state if passive event occures
    public string passiveAiState;


    // AI behavior of this object
    private AiBehavior aiBehavior;
    // Target for attack
    private GameObject target;
    // List with potential targets finded during this frame
    private List<GameObject> targetsList = new List<GameObject>();
    // My melee attack type if it is
    private IAttack meleeAttack;
    // My ranged attack type if it is
    private IAttack rangedAttack;

    private IAttack magicAttack;
    // Type of last attack is made
    private IAttack myLastAttack;
    // My navigation agent if it is
    NavAgent nav;
    // Allows to await new target for one frame before exit from this state
    private bool targetless;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        this.aiBehavior   = this.GetComponent<AiBehavior> ();
        this.meleeAttack  = this.GetComponentInChildren<AttackMelee>();
        this.rangedAttack = this.GetComponentInChildren<AttackRanged>();
        this.magicAttack  = this.GetComponentInChildren<AttackMagic>();
        this.nav          = this.GetComponent<NavAgent>();
        Debug.Assert ((this.aiBehavior != null) && ((this.meleeAttack != null) || (this.rangedAttack != null) || (this.magicAttack != null)), "Wrong initial parameters");
    }

    /// <summary>
    /// Raises the state enter event.
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
    public void OnStateEnter (string previousState, string newState)
    {

    }

    /// <summary>
    /// Raises the state exit event.
    /// </summary>
    /// <param name="previousState">Previous state.</param>
    /// <param name="newState">New state.</param>
    public void OnStateExit (string previousState, string newState)
    {
        this.LoseTarget();
    }

    /// <summary>
    /// FixedUpdate for this instance.
    /// </summary>
    void FixedUpdate ()
    {
        // If have no target - try to get new target
        if ((this.target == null) && (this.targetsList.Count > 0))
        {
            this.target = this.GetTopmostTarget();
            if ((this.target != null) && (this.nav != null))
            {
                // Look at target
                this.nav.LookAt(this.target.transform);
            }
        }
        // There are no targets around
        if (this.target == null)
        {
            if (this.targetless == false)
            {
                this.targetless = true;
            }
            else
            {
                // If have no target more than one frame - exit from this state
                this.aiBehavior.ChangeState(this.passiveAiState);
            }
        }
    }

    /// <summary>
    /// Gets top priority target from list.
    /// </summary>
    /// <returns>The topmost target.</returns>
    private GameObject GetTopmostTarget()
    {
        GameObject res = null;
        if (this.useTargetPriority == true) // Get target with minimum distance to capture point
        {
            float minPathDistance = float.MaxValue;
            foreach (GameObject ai in this.targetsList)
            {
                if (ai != null)
                {
                    AiStatePatrol aiStatePatrol = ai.GetComponent<AiStatePatrol>();
                    float distance = aiStatePatrol.GetRemainingPath();
                    if (distance < minPathDistance)
                    {
                        minPathDistance = distance;
                        res = ai;
                    }
                }
            }
        }
        else // Get first target from list
        {
            res = this.targetsList[0];
        }
        // Clear list of potential targets
        this.targetsList.Clear();
        return res;
    }

    /// <summary>
    /// Loses the current target.
    /// </summary>
    private void LoseTarget()
    {
        this.target     = null;
        this.targetless = false;
        this.myLastAttack  = null;
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
        if (this.target == null) // Add new target to potential targets list
        {
            this.targetsList.Add(other.gameObject);
        }
        else // Attack current target
        {
            // If this is my current target
            if (this.target == other.gameObject)
            {
                if (my.name == "MeleeAttack") // If target is in melee attack range
                {
                    // If I have melee attack type
                    if (this.meleeAttack != null)
                    {
                        // Remember my last attack type
                        this.myLastAttack = this.meleeAttack;
                        // Try to make melee attack
                        this.meleeAttack.Attack(other.transform);
                    }
                }
                else if (my.name == "RangedAttack") // If target is in ranged attack range
                {
                    // If I have ranged attack type
                    if (this.rangedAttack != null)
                    {
                        // If target not in melee attack range
                        if ((this.meleeAttack == null)
                            || ((this.meleeAttack != null) && (this.myLastAttack != this.meleeAttack)))
                        {
                            // Remember my last attack type
                            this.myLastAttack = this.rangedAttack;
                            // Try to make ranged attack
                            this.rangedAttack.Attack(other.transform);
                        }
                    }
                }
                else if (my.name == "MagicAttack")
                {
                    if (this.magicAttack != null)
                    {
                        // If target not in melee attack range
                        if ((this.meleeAttack == null)
                            || ((this.meleeAttack != null) && (this.myLastAttack != this.meleeAttack)))
                        {
                            // Remember my last attack type
                            this.myLastAttack = this.magicAttack;
                            // Try to make ranged attack
                            this.magicAttack.Attack(other.transform);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Triggers the exit.
    /// </summary>
    /// <param name="my">My.</param>
    /// <param name="other">Other.</param>
    public void TriggerExit(Collider2D my, Collider2D other)
    {
        if (other.gameObject == this.target)
        {
            // Lose my target if it quit attack range
            this.LoseTarget();
        }
    }
}
