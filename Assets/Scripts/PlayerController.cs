using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // Initialize target position to the player's start position, and lock Z-axis at -0.1
        targetPosition = new Vector3(transform.position.x, transform.position.y, -0.1f);
        transform.position = targetPosition; // Ensure player starts with correct Z position
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (isMoving)
        {
            MovePlayer();
        }
        else if (Input.GetMouseButtonDown(0)) // Left click for movement
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 target = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.1f); // Lock Z-axis

            if (IsTileOccupied(target))
            {
                return; // Tile is occupied, do not move to it
            }

            targetPosition = target;
            isMoving = true;
        }
    }

    void MovePlayer()
    {
        // Move player towards the target position while keeping the Z position locked at -0.1
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Ensure the Z-axis remains locked
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    bool IsTileOccupied(Vector3 targetPos)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(targetPos, 0.1f); // Check small area around target position
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("NPC"))
            {
                Debug.Log("NPC detected at target position!"); // Print to console when NPC is detected
                return true; // Tile is occupied by an NPC
            }
        }
        return false;
    }
}