using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainIncreasesScript : MonoBehaviour
{
    [SerializeField] private GameObject rainfall;
    private ParticleSystem particleSystem;

    // A variable to store the original rate
    private float initialRate;

    public void Start()
    {
        particleSystem = rainfall.GetComponent<ParticleSystem>();
        // Store the starting emission rate
        initialRate = particleSystem.emission.rateOverTime.constant;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var emission = particleSystem.emission;
            // Set the rate to 1.5x the original rate
            emission.rateOverTime = initialRate * 1.5f;
            Destroy(gameObject);
        }
    }
}

