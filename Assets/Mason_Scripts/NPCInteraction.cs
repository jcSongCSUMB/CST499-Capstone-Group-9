using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;
using System.Linq;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI choiceText;
    public GameObject dialogueBox;
    public GameObject choiceBox;

    private string[] dialogueLines;
    private int dialogueIndex = 0;
    private bool isInteracting = false;
    private bool isTyping = false;
    private bool isChoosing = false;

    private string currentLine = "";
    private string characterName = "";
    private int currentChoiceIndex = 0;
    private string[] currentChoices;

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
            else if (isInteracting)
            {
                if (isChoosing)
                {
                    ConfirmChoice();
                }
                else if (isTyping)
                {
                    // Skip typing and display the full line
                    StopAllCoroutines();
                    dialogueText.text = characterName + currentLine; // Preserve the name and full dialogue
                    isTyping = false;
                }
                else
                {
                    DisplayNextLine();
                }
            }
        }

        if (isChoosing)
        {
            HandleChoiceNavigation();
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
                // Display normal dialogue
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
            characterName = line.Substring(0, delimiterIndex + 1) + " "; // Keep the name + ": "
            currentLine = line.Substring(delimiterIndex + 2); // Keep the dialogue text
        }
        else
        {
            characterName = "";
            currentLine = line;
        }

        // Set initial text to the name only
        dialogueText.text = characterName;

        // Gradually display the dialogue text
        StartCoroutine(TypeText(currentLine));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        // Start with the character's name
        dialogueText.text = characterName;

        // Gradually append letters from the dialogue text
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        // End typing
        isTyping = false;
    }

    void ShowChoices()
    {
        choiceBox.SetActive(true);
        choiceText.enabled = true;

        // Extract choices by skipping and removing the "Player (Choice): " prefix
        currentChoices = dialogueLines
            .Skip(dialogueIndex) // Start from current index
            .TakeWhile(line => line.StartsWith("Player (Choice):")) // Only take choice lines
            .Select(line => line.Replace("Player (Choice): ", "").Trim()) // Remove prefix
            .ToArray();

        // Update dialogue index to skip choice lines after selection
        dialogueIndex += currentChoices.Length;

        UpdateChoiceDisplay();
    }

    void UpdateChoiceDisplay()
    {
        string choiceDisplay = "";
        for (int i = 0; i < currentChoices.Length; i++)
        {
            if (i == currentChoiceIndex)
            {
                choiceDisplay += $"<u>{currentChoices[i]}</u>\n";
            }
            else
            {
                choiceDisplay += $"{currentChoices[i]}\n";
            }
        }
        choiceText.text = choiceDisplay.TrimEnd();
    }

    void HandleChoiceNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentChoiceIndex = Mathf.Max(0, currentChoiceIndex - 1);
            UpdateChoiceDisplay();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentChoiceIndex = Mathf.Min(currentChoices.Length - 1, currentChoiceIndex + 1);
            UpdateChoiceDisplay();
        }
    }

    void ConfirmChoice()
    {
        isChoosing = false;
        choiceBox.SetActive(false);
        choiceText.enabled = false;

        dialogueIndex += 2; // Skip over the choice lines
        DisplayNextLine();
    }

    void EndDialogue()
    {
        dialogueText.enabled = false;
        dialogueBox.SetActive(false);
        isInteracting = false;
    }
}
