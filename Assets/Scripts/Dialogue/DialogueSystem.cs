using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject continueIndicator;
    
    public bool useTypingEffect = true;
    public float typingSpeed = 0.05f;
    public float inputCooldown = 0.2f; // Cooldown between inputs
    
    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isTyping = false;
    private Coroutine typingRoutine;
    private float lastInputTime = -1f;
    private bool canAcceptInput = true;
    
    private void Start()
    {
        if(continueIndicator != null)
            continueIndicator.SetActive(false);
    }
    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueBox.SetActive(true);
        canAcceptInput = true;
        lastInputTime = Time.time;
        ShowLine(currentLineIndex);
        Debug.Log("Dialogue Started");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canAcceptInput)
        {
            if (Time.time - lastInputTime >= inputCooldown)
            {
                OnNextButtonPressed();
                lastInputTime = Time.time;
            }
        }
    }
    public void ForceCompleteDialogue()
    {
        if (canAcceptInput)
        {
            OnNextButtonPressed();
        }
    }
    private void ShowLine(int lineIndex)
    {
        if (lineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }
        
        var line = currentDialogue.lines[lineIndex];
        if(continueIndicator != null)
            continueIndicator.SetActive(false);
        if (useTypingEffect)
        {
            if(typingRoutine != null)
                StopCoroutine(typingRoutine);
            typingRoutine = StartCoroutine(TypeText(line.text));
        }
        else
        {
            dialogueText.text = line.text;
            isTyping = false;
            if(continueIndicator != null)
                continueIndicator.SetActive(true);
        }
    }

    
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (var c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        if(continueIndicator != null)
            continueIndicator.SetActive(true);
        
    }
    
    private void EndDialogue()
    {
        canAcceptInput = false;
        dialogueBox.SetActive(false);
        if(continueIndicator != null)
            continueIndicator.SetActive(false);
        
    }
    
    private void OnNextButtonPressed()
    {
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            isTyping = false;
            if(continueIndicator != null)
                continueIndicator.SetActive(true);
        }
        else
        {
            currentLineIndex++;
            ShowLine(currentLineIndex);
        }
    }
}
