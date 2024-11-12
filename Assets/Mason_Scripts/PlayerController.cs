using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Queue<Vector3> path = new Queue<Vector3>();
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // Initialize target position to the player's start position, and lock Z-axis at -0.1
        targetPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), -0.1f);
        transform.position = targetPosition; // Ensure player starts with correct Z position
    }

    void Update()
    {
        if (isMoving)
        {
            MovePlayerAlongPath();
        }
        else if (Input.GetMouseButtonDown(0)) // Left click for movement
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 target = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.1f); // Lock Z-axis

            if (target != targetPosition) // Check if the target is different from the current position
            {
                path = CalculatePath(transform.position, target); // Calculate path
                if (path.Count > 0)
                {
                    targetPosition = path.Dequeue(); // Set next target position from the path
                    isMoving = true;
                }
            }
        }
    }

    void MovePlayerAlongPath()
    {
        if (path.Count == 0)
        {
            isMoving = false;
            return;
        }

        // Move towards the next target position along the path
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the player has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition; // Snap to the target position

            if (path.Count > 0)
            {
                targetPosition = path.Dequeue(); // Set the next target position
            }
            else
            {
                isMoving = false; // Reached the final destination
            }
        }
    }

    Queue<Vector3> CalculatePath(Vector3 start, Vector3 target)
    {
        Queue<Vector3> path = new Queue<Vector3>();
        HashSet<Vector3> visited = new HashSet<Vector3>();
        Queue<Vector3> frontier = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

        // Round the start and target positions to ensure they align with the grid
        start = new Vector3(Mathf.Round(start.x), Mathf.Round(start.y), -0.1f);
        target = new Vector3(Mathf.Round(target.x), Mathf.Round(target.y), -0.1f);

        // Initialize BFS
        frontier.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = start;

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        bool targetReached = false;

        // Breadth-first search (BFS)
        while (frontier.Count > 0)
        {
            Vector3 current = frontier.Dequeue();

            // Check if we've reached the target
            if (current == target)
            {
                targetReached = true;
                break;
            }

            // Explore neighbors in non-diagonal directions
            foreach (Vector3 direction in directions)
            {
                Vector3 neighbor = current + direction;

                // Check if neighbor is visited and not occupied
                if (!visited.Contains(neighbor) && !IsTileOccupied(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (targetReached)
        {
            // Reconstruct the path if target was reached
            Vector3 step = target;

            // Reconstruct path from target to start
            while (step != start)
            {
                if (cameFrom.ContainsKey(step))
                {
                    path.Enqueue(step);
                    step = cameFrom[step];
                }
                else
                {
                    Debug.LogError($"Path reconstruction failed. Missing key in cameFrom for position: {step}");
                    break;
                }
            }
            path.Enqueue(start);  // Include start position
            path = new Queue<Vector3>(path.Reverse()); // Reverse path for correct order
        }
        else
        {
            Debug.LogWarning("No path found to the target. Returning an empty path.");
        }

        return path;
    }

    bool IsTileOccupied(Vector3 targetPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPosition, 0.1f);  // Adjust radius as needed
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("NPC") || collider.CompareTag("Obstacle"))  // Check if there's an obstacle or NPC
            {
                return true;
            }
        }
        return false;
    }
}