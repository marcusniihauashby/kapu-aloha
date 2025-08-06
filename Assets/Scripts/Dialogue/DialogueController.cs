using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameTextUI;
    [SerializeField] private TextMeshProUGUI dialogueTextUI;
    [SerializeField] private float typeSpeed = 20f;

    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    private Queue<Tuple<String, String>> paragraphs = new Queue<Tuple<string, string>>();

    private bool conversationEnded;
    private bool isTyping;
    private string n;
    private string p;

    private Coroutine typeDialogueCoroutine;

    private const string HTML_ALPHA = "<color=#00000000>";

    public const float IDLE_TIME_BETWEEN_DIALOGUE = 4f;
    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                // start conversation
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                // end conversation
                EndConversation();
                return;
            }
        }
        if (!isTyping)
        {
            var tup = paragraphs.Dequeue();
            n = tup.Item1;
            p = tup.Item2;

            nameTextUI.text = n;

            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }

    }

    private void StartConversation(DialogueText dialogueText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(Tuple.Create(dialogueText.speakerName[i], dialogueText.paragraphs[i]));
        }
    }
    private void EndConversation()
    {
        // clear the queue
        paragraphs.Clear();

        // set conversationEnded to false
        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    public void HandleConversation(DialogueText dialogueText)
    {
        // stop player movement until this is over
        DisplayNextParagraph(dialogueText);
        
        
    }
        public void HandleMonologue(DialogueText dialogueText)
    {
        DisplayNextParagraph(dialogueText);
        StartCoroutine(RunMonologue(dialogueText));

    }

    private IEnumerator RunMonologue(DialogueText dialogueText)
    {
        while (!conversationEnded)
        {
            yield return new WaitUntil(() => !isTyping);
            yield return new WaitForSeconds(IDLE_TIME_BETWEEN_DIALOGUE);
            DisplayNextParagraph(dialogueText);
        }
        // --- FIX STARTS HERE ---

        // 1. Wait for the final paragraph to finish typing.
        yield return new WaitUntil(() => !isTyping);

        // 2. (Optional) Wait a moment so the player can read the last line.
        yield return new WaitForSeconds(IDLE_TIME_BETWEEN_DIALOGUE);

        // 3. Now, properly end the conversation and close the UI.
        DisplayNextParagraph(dialogueText);
    }

    

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        dialogueTextUI.text = "";

        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            dialogueTextUI.text = originalText;

            displayedText = dialogueTextUI.text.Insert(alphaIndex, HTML_ALPHA);
            dialogueTextUI.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        if (typeDialogueCoroutine != null)
            StopCoroutine(typeDialogueCoroutine);

        dialogueTextUI.text = p;

        isTyping = false;
    }
}
