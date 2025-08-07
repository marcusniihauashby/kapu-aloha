using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GettingCaught : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip lightningSound;
    public EasyPeasyFirstPersonController.FirstPersonController playerObject;
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private Vector3 respawnRotation;
    [SerializeField] private Vector3 boarRespawnPoint;
    [SerializeField] private Vector3 farAwayPoint;
    private bool isCaught = false;

    [SerializeField] private GameObject blackOverlay;

    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;

    [SerializeField] private GameObject[] ambienceManagers;
    

    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCaught && other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Entered Trigger");
            isCaught = true;
            // move boar (parent of this object) back to it's initial spawn location
            StartCoroutine(HandleCaught());
        }
    }

    private IEnumerator HandleCaught()
    {
        // save player movement speed, so I can disable it and change it back
        float originalWalkSpeed = playerObject.walkSpeed;
        float originalSprintSpeed = playerObject.sprintSpeed;

        //move boar far away (some random far away point i picked so it doesnt make sound anymore)
        transform.parent.position = farAwayPoint;


        // activate gameover screen.
        blackOverlay.SetActive(true);

        // turn off ambient sound
        float[] savedVolumes = new float[ambienceManagers.Length];

        for (int i = 0; i < ambienceManagers.Length; i++)
        {
            GameObject ambienceManager = ambienceManagers[i];
            AudioSource source = ambienceManager.GetComponent<AudioSource>();
            savedVolumes[i] = source.volume;
            source.volume = 0f;
        }


        // disable player movement
        playerObject.walkSpeed = 0;
        playerObject.sprintSpeed = 0;

        // Optional: Add short delay for sound or screen fade
        yield return new WaitForSeconds(0.5f);
        // Play attack sound effect
        SoundFXManager.instance.PlaySoundFXClip(deathSound, playerObject.transform.position, 1f);
        yield return new WaitForSeconds(4f);

        // play dialogue
        dialogueController.HandleMonologue(dialogueText);

        // Thanks gemini
        yield return new WaitUntil(() => !dialogueController.gameObject.activeInHierarchy);

        // Move player back to checkpoint
        playerObject.transform.position = respawnPoint;


        //play lightning sound effect
        SoundFXManager.instance.PlaySoundFXClip(lightningSound, playerObject.transform.position, 1f);

        //white flash (maybe)

        // remove gameover screen
        blackOverlay.SetActive(false);

        // re-enable player movement
        isCaught = false;
        playerObject.walkSpeed = originalWalkSpeed;
        playerObject.sprintSpeed = originalSprintSpeed;

        // turn ambient sound back on
        for (int i = 0; i < ambienceManagers.Length; i++)
        {
            GameObject ambienceManager = ambienceManagers[i];
            AudioSource source = ambienceManager.GetComponent<AudioSource>();
            source.volume = savedVolumes[i];
        }

        // return boar to where it belongs
        transform.parent.position = boarRespawnPoint;
    }
}

