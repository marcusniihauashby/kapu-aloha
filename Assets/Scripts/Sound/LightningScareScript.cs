using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScareScript : MonoBehaviour
{
    // Start is called before the first frame update

    private bool soundPlayed;
    private MeshRenderer meshRenderer;
    [SerializeField] private GameObject dialogueTrigger;

    [SerializeField] private AudioClip lightningSound;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (meshRenderer.enabled == false && !soundPlayed)
        {
            SoundFXManager.instance.PlaySoundFXClip(lightningSound, transform.position, 1f);
            soundPlayed = true;
            dialogueTrigger.SetActive(true);
        }
    }
}
