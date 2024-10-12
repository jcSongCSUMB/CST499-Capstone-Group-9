using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    private string[] dialogueLines = {
        "This is line 1.",
        "This is line 2.",
        "This is line 3."
    };

    private int dialogueIndex = 0;
    private bool isInteracting = false;

    void Start()
    {
        dialogueBox.SetActive(false); // Hide the dialogue box initially
    }

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        // Only check for interaction if the player is near an NPC
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (IsPlayerAdjacent() && !isInteracting)
            {
                StartDialogue();
            }
            else if (isInteracting)
            {
                DisplayNextLine();
            }
        }
    }

    bool IsPlayerAdjacent()
    {
        // Check if the player is adjacent to the NPC
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        Vector3 npcPosition = transform.position;

        // Calculate distance between player and NPC on the X and Y axes
        float distanceX = Mathf.Abs(playerPosition.x - npcPosition.x);
        float distanceY = Mathf.Abs(playerPosition.y - npcPosition.y);

        // Return true if the player is on an adjacent tile
        return (distanceX <= 1f && distanceY <= 1f);
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
            Debug.Log($"Displaying line {dialogueIndex + 1}: {dialogueLines[dialogueIndex]}"); // Print to console
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