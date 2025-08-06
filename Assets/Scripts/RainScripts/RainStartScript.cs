using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rainSoundsToActivate;
    [SerializeField] private GameObject rainToActivate;
    private AudioSource audioSource;

    public float amountToChange = 0.25f;
    public float timeToFinishChangingAudio = 1.5f;
    public void Start()
    {
        audioSource = rainSoundsToActivate.GetComponent<AudioSource>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rainToActivate.SetActive(true);
            // i dont give a fuck im starting a coroutine and not making my system nice and modular
            StartCoroutine(ChangeVolume(amountToChange, timeToFinishChangingAudio));
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    IEnumerator ChangeVolume(float volumeDifference, float seconds)
    {
        float startVolume = audioSource.volume;
        float targetVolume = Mathf.Clamp(startVolume + volumeDifference, 0f, 1f);
        float elapsedTime = 0f;

        while (elapsedTime < seconds)
        {
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

}
