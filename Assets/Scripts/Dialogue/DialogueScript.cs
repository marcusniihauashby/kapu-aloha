using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueScript : MonoBehaviour, IDialogue
{
    // a file consisting of array of names and array of paragraphs.
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;

    [SerializeField] private AudioClip audioClip;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioClip != null)
            {                
                SoundFXManager.instance.PlaySoundFXClip(audioClip, transform.position, 1f);
            }
            if (dialogueText.isConversation)
            {
                Conversation(dialogueText);
            }
            else
            {
                Monologue(dialogueText);
            }

        }
    }
    public void Monologue(DialogueText dialogueText)
    {
        dialogueController.HandleMonologue(dialogueText);
    }

    public void Conversation(DialogueText dialogueText)
    {
        dialogueController.HandleConversation(dialogueText);
    }
}
