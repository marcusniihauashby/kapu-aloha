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
    private bool isCaught = false;

    [SerializeField] private GameObject blackOverlay;

    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;

    [SerializeField] private GameObject ambienceManager;

    void Start()
    {
        GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isCaught && other.CompareTag("Player"))
        {
            isCaught = true;
            StartCoroutine(HandleCaught());
        }
    }

    private IEnumerator HandleCaught()
    {
        // save player movement speed, so I can disable it and change it back
        float originalWalkSpeed = playerObject.walkSpeed;
        float originalSprintSpeed = playerObject.sprintSpeed;


        // activate gameover screen.
        blackOverlay.SetActive(true);

        // turn off ambient sound
        AudioSource source = ambienceManager.GetComponent<AudioSource>();
        float originalVolume = source.volume;
        source.volume = 0f;


        // disable player movement
        playerObject.walkSpeed = 0;
        playerObject.sprintSpeed = 0;

        // Optional: Add short delay for sound or screen fade
        yield return new WaitForSeconds(0.5f);
        // Play attack sound effect
        SoundFXManager.instance.PlaySoundFXClip(deathSound, playerObject.transform.position, 1f);
        yield return new WaitForSeconds(6f);

        // play dialogue
        dialogueController.HandleMonologue(dialogueText);



        // Thanks gemini
        Time.timeScale = 1;
        yield return new WaitUntil(() => !dialogueController.gameObject.activeInHierarchy);

        // Move player back to checkpoint
        playerObject.transform.position = respawnPoint;

        UnityEngine.Debug.Log("Waiting 1 seconds...");
        // for some reason this breaks everything, and it never gets to finished waiting. when i remove it it's completely fine.
        yield return new WaitForSeconds(1f);
        UnityEngine.Debug.Log("Finished waiting!");

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
        source.volume = originalVolume;

    }
}

