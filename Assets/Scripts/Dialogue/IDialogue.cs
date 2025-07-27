using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogue
{
    public void Monologue(DialogueText dialogueText);

    public void Conversation(DialogueText dialogueText);
}
