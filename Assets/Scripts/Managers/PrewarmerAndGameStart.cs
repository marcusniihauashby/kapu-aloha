using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrewarmer : MonoBehaviour
{
    // Drag your DialogueController's GameObject here in the Inspector
    [SerializeField] private GameObject dialogueCanvasObject;

    void Start()
    {
        // This forces Unity to do all the expensive first-time setup now
        dialogueCanvasObject.SetActive(true);
        
        // Then we immediately hide it so the player doesn't see it
        dialogueCanvasObject.SetActive(false);
    }
}