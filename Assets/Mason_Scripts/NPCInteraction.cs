using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;
using System.Linq;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;   // Text for dialogue box
    public TextMeshProUGUI choiceText;     // Text for choice box
    public GameObject dialogueBox;         // Dialogue box UI object
    public GameObject choiceBox;           // Choice box UI object

    private string[] dialogueLines;        // Loaded dialogue lines
    private int dialogueIndex = 0;         // Tracks current dialogue line
    private bool isInteracting = false;    // Tracks if interaction is active
    private bool isTyping = false;         // Tracks if text is typing
    private bool isChoosing = false;       // Tracks if player is making a choice

    private string currentLine = "";       // Current dialogue line being typed
    private string characterName = "";     // Stores the speaker's name

    void Start()
    {
        dialogueText.enabled = false;
        dialogueBox.SetActive(false);
        choiceText.enabled = false;
        choiceBox.SetActive(false);
    }

    void Update()
    {
        HandleInteraction();
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (IsPlayerAdjacent() && !isInteracting)
            {
                StartDialogue();
            }
            else if (isInteracting && !isChoosing)
            {
                if (isTyping)
                {
                    StopAllCoroutines();
                    dialogueText.text = currentLine;
                    isTyping = false;
                }
                else
                {
                    DisplayNextLine();
                }
            }
        }

        // Handle choice navigation
        if (isChoosing)
        {
            HandleChoiceInput();
        }
    }

    bool IsPlayerAdjacent()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
        Vector3 npcPosition = transform.position;

        float distanceX = Mathf.Abs(playerPosition.x - npcPosition.x);
        float distanceY = Mathf.Abs(playerPosition.y - npcPosition.y);

        return (distanceX <= 1f && distanceY <= 1f);
    }

    void StartDialogue()
    {
        dialogueIndex = 0;
        isInteracting = true;

        // Load dialogue lines from file
        string npcName = gameObject.name;
        string filePath = $"Assets/Quest Scripts/{npcName.Replace("NPC", "")}.txt";
        dialogueLines = File.ReadAllLines(filePath);

        dialogueText.enabled = true;
        dialogueBox.SetActive(true);
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (dialogueIndex < dialogueLines.Length)
        {
            string line = dialogueLines[dialogueIndex];

            if (line.StartsWith("Player (Choice):"))
            {
                // Enter choice mode
                isChoosing = true;
                ShowChoices();
            }
            else
            {
                // Display dialogue
                ParseLine(line);
                dialogueIndex++;
            }
        }
        else
        {
            EndDialogue();
        }
    }

    void ParseLine(string line)
    {
        StopAllCoroutines();

        // Split line into name and text
        int delimiterIndex = line.IndexOf(": ");
        if (delimiterIndex > 0)
        {
            characterName = line.Substring(0, delimiterIndex + 2);
            currentLine = line.Substring(delimiterIndex + 2);
        }
        else
        {
            characterName = "";
            currentLine = line;
        }

        // Immediately display the character name
        dialogueText.text = characterName;

        // Gradually display the rest of the text
        StartCoroutine(TypeText(currentLine));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

    void ShowChoices()
    {
        choiceBox.SetActive(true);
        choiceText.enabled = true;

        // Extract and display choices
        string[] choices = dialogueLines
            .Skip(dialogueIndex)
            .TakeWhile(line => line.StartsWith("Player (Choice):"))
            .Select(line => line.Replace("Player (Choice): ", ""))
            .ToArray();

        choiceText.text = string.Join("\n", choices);
    }

    void HandleChoiceInput()
    {
        // Add navigation logic for choices (e.g., highlight with arrow keys)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Highlight previous choice
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Highlight next choice
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // Confirm choice
            isChoosing = false;
            choiceBox.SetActive(false);
            choiceText.enabled = false;

            // Resume dialogue after choice
            dialogueIndex++;  // Skip over the Player's choice line
            DisplayNextLine();
        }
    }

    void EndDialogue()
    {
        dialogueText.enabled = false;
        dialogueBox.SetActive(false);
        isInteracting = false;
    }
}