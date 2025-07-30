using System.Collections;
using System.Collections.Generic;
using EasyPeasyFirstPersonController;
using UnityEngine;

public class UIPrewarmer : MonoBehaviour
{
    // Drag your DialogueController's GameObject here in the Inspector
    [SerializeField] private GameObject dialogueCanvasObject;
    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();


        // Hardcoded value for turning player around. Dont worry about it.
        playerObject.RotateInstantly(180f);
        
        // This forces Unity to do all the expensive first-time setup now
        dialogueCanvasObject.SetActive(true);
        
        // Then we immediately hide it so the player doesn't see it
        dialogueCanvasObject.SetActive(false);
    }
}