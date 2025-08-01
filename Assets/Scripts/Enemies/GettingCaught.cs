using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingCaught : MonoBehaviour
{
    public AudioClip caughtSound;
    public Transform player;
    public Transform respawnPoint; // Assign checkpoint here

    private AudioSource audioSource;
    private bool isCaught = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        // Play sound
        audioSource.PlayOneShot(caughtSound);

        // Optional: Add short delay for sound or screen fade
        yield return new WaitForSeconds(1f);

        // Move player back to checkpoint
        player.position = respawnPoint.position;
        player.rotation = respawnPoint.rotation;

        isCaught = false;
    }
}

