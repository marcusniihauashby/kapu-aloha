using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rainSoundsToActivate;
    [SerializeField] private GameObject rainToActivate;
    private AudioSource audioSource;
    public void Start()
    {
        audioSource = rainSoundsToActivate.GetComponent<AudioSource>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rainToActivate.SetActive(true);
            Destroy(gameObject);
            // TODO: TURN ON RAIN SOUNDS
            audioSource.volume += 0.1f;

        }
    }
}
