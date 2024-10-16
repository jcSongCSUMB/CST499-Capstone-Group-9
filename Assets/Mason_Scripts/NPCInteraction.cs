using UnityEngine;
using TMPro;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    private string[] dialogueLines = {
        "This is line 1.",
        "This is line 2.",
        "This is line 3."
    };
    
    public static bool isInteracting = false;
    private bool isTyping = false;  // Track if a line is currently being typed
    private string currentLine = "";  // Track the current line being typed
    private int dialogueIndex = 0;

    void Start()
    {
        // Initially hide dialogue objects from player view
        dialogueText.enabled = false;
        dialogueBox.SetActive(false);
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
                if (isTyping)
                {
                    // If typing is in progress, instantly show the full line
                    StopAllCoroutines();
                    dialogueText.text = currentLine;
                    isTyping = false;  // Line is now fully displayed
                }
                else
                {
                    // Move to the next line
                    DisplayNextLine();
                }
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
        dialogueText.enabled = true;
        dialogueBox.SetActive(true);
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (dialogueIndex < dialogueLines.Length)
        {
            currentLine = dialogueLines[dialogueIndex];  // Store the current line
            StopAllCoroutines();  // Ensure no previous coroutine is running
            StartCoroutine(TypeText(currentLine));
            dialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeText(string fullLine)
    {
        isTyping = true;  // Set flag to true, indicating typing is in progress
        dialogueText.text = "";  // Clear current text
        foreach (char letter in fullLine.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);  // Adjust this delay for typing speed
        }
        isTyping = false;  // Set flag to false, typing has finished
    }

    void EndDialogue()
    {
        dialogueText.enabled = false;
        dialogueBox.SetActive(false);
        isInteracting = false;
    }
}
