using Signals;
using UnityEngine;
using Zenject;

/// <summary>
/// Moving and turning operation.
/// </summary>
public class NavAgent : MonoBehaviour
{
    // Speed im m/s
    public float speed = 1f;

    // Can moving
    public bool move = true;

    // Can turning
    public bool turn = true;

    // Destination position
    [HideInInspector] public Vector2 destination;

    // Velocity vector
    [HideInInspector] public Vector2 velocity;

    // Position on last frame
    private Vector2 prevPosition;
  
    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        this.prevPosition = this.transform.position;
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        // If moving is allowed
        if (this.move)
        {
            // Move towards destination point
            this.transform.position = Vector2.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);
        }

        // Calculate velocity
        Vector2 velocity = (Vector2)this.transform.position - this.prevPosition;
        velocity /= Time.deltaTime;
        // If turning is allowed
        if (this.turn == true)
        {
            this.SetSpriteDirection(this.destination - (Vector2)this.transform.position);
        }

        // Save last position
        this.prevPosition = this.transform.position;
    }

    /// <summary>
    /// Sets sprite direction on x axis.
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void SetSpriteDirection(Vector2 direction)
    {
        if (direction.x > 0f && this.transform.localScale.x < 0f) // To the right
        {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (direction.x < 0f && this.transform.localScale.x > 0f) // To the left
        {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }
    }

    /// <summary>
    /// Looks at direction.
    /// </summary>
    /// <param name="direction">Direction.</param>
    public void LookAt(Vector2 direction) { this.SetSpriteDirection(direction); }

    /// <summary>
    /// Looks at target.
    /// </summary>
    /// <param name="target">Target.</param>
    public void LookAt(Transform target) { this.SetSpriteDirection(target.position - this.transform.position); }
}