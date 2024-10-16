using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = new Vector3(transform.position.x, transform.position.y, -0.1f);
        transform.position = targetPosition;
    }

    void Update()
    {
        // Prevent movement if interacting with an NPC
        if (!NPCInteraction.isInteracting)
        {
            HandleMovement();
        }
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
            Vector3 target = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), -0.1f); 

            if (IsTileOccupied(target))
            {
                return;
            }

            targetPosition = target;
            isMoving = true;
        }
    }

    void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Ensure Z position remains locked
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    bool IsTileOccupied(Vector3 targetPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPosition, 0.1f);  
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                return true;
            }
        }
        return false;
    }
}
