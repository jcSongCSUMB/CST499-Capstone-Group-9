using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public float moveSpeed = 5f;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private string[] dialogueLines = {
        "Placeholder text for line 1.",
        "Placeholder text for line 2.",
        "Placeholder text for line 3."
    };

    private int dialogueIndex = 0;
    private bool isInteracting = false;

    void Start()
    {
        targetPosition = transform.position; // Start at the current position
        dialogueBox.SetActive(false); // Hide dialogue box initially
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
        targetPosition = new Vector3(transform.position.x, transform.position.y, -0.1f);
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
            Vector3 target = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);

            if (IsTileOccupied(target))
            {
                // If the tile is occupied, stop the movement attempt
                return;
            }

            targetPosition = target;
            isMoving = true;
        }
    }

    void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    bool IsTileOccupied(Vector3 targetPos)
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(targetPos, 0.1f); // Check small area for colliders
        if (hitCollider != null && hitCollider.CompareTag("NPC"))
        {
            return true; // Tile is occupied by an NPC
        }
        return false;
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (IsNPCNearby() && !isInteracting)
            {
                StartDialogue();
            }
            else if (isInteracting)
            {
                DisplayNextLine();
            }
        }
    }

    bool IsNPCNearby()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f); // Check for nearby NPCs
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("NPC"))
            {
                return true; // NPC is in range
            }
        }
        return false;
    }

    void StartDialogue()
    {
        isInteracting = true;
        dialogueIndex = 0;
        dialogueBox.SetActive(true);
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (dialogueIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        isInteracting = false;
    }
}
