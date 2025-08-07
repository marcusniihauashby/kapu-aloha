using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScareScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool soundPlayed;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    [SerializeField] private GameObject dialogueTrigger;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (meshRenderer.enabled == false && !soundPlayed)
        {
            // SoundFXManager.instance.PlaySoundFXClip(lightningSound, transform.position, 1f);
            audioSource.Play();
            soundPlayed = true;
            dialogueTrigger.SetActive(true);
        }
    }
}
