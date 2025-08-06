using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAmbienceVolume : MonoBehaviour
{

    [SerializeField] GameObject audioManager;
    public float amountChanged = 0f;

    public float timeToFinishChangingAudio = 5f;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private bool soundLowered = false;



    void Start()
    {
        audioSource = audioManager.GetComponent<AudioSource>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }
    void Update()
    {
        if (!meshRenderer.enabled && !soundLowered)
        {
            soundLowered = true;
            StartCoroutine(ChangeVolume(amountChanged, timeToFinishChangingAudio));
        }
    }

    IEnumerator ChangeVolume(float volumeDifference, float seconds)
    {
        float startVolume = audioSource.volume; // Get the current volume as the starting point
        float targetVolume = startVolume + volumeDifference; // Calculate the target volume

        // Clamp the target volume to ensure it stays within the valid range [0, 1]
        targetVolume = Mathf.Clamp(targetVolume, 0f, 1f);

        float elapsedTime = 0f; // Initialize a timer for the elapsed time

        // Loop while the elapsed time is less than the total duration
        while (elapsedTime < seconds)
        {
            // Calculate the current volume using Mathf.Lerp (Linear Interpolation)
            // This smoothly moves the volume from startVolume to targetVolume
            // based on the progress (elapsedTime / seconds)
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / seconds);

            elapsedTime += Time.deltaTime; // Increment the elapsed time by the time since the last frame
            yield return null; // Wait for the next frame before continuing the loop
        }

        // After the loop, ensure the volume is exactly the target volume to avoid floating point inaccuracies
        audioSource.volume = targetVolume;
    }
}
