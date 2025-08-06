using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainIncreasesScript : MonoBehaviour
{
    [SerializeField] private GameObject rainfall;
    private ParticleSystem particleSystem;

    [SerializeField] private GameObject rainAudioManager;

    private AudioSource audioSource;

    public float amountToChange = 0.1f;

    public float timeToFinishChangingAudio = 5f;

    // A variable to store the original rate
    private float initialRate;

    public void Start()
    {
        particleSystem = rainfall.GetComponent<ParticleSystem>();
        // Store the starting emission rate
        initialRate = particleSystem.emission.rateOverTime.constant;
        audioSource = rainAudioManager.GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var emission = particleSystem.emission;
            // Set the rate to 1.5x the original rate
            emission.rateOverTime = initialRate * 2f;
            Destroy(gameObject);
            StartCoroutine(ChangeVolume(amountToChange, timeToFinishChangingAudio));
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

