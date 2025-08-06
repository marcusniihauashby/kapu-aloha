using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Vector3 spawnTransform, float volume)
    {
        //spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform, Quaternion.identity);
        //assign the AudioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();


        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy GameObject when the sound FX clip is done.
        Destroy(audioSource.gameObject, clipLength);
    }


    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Vector3 spawnTransform, float volume)
    {
        //Find random number
        int rand = Random.Range(0, audioClips.Length);

        //spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform, Quaternion.identity);

        //assign the AudioClip
        audioSource.clip = audioClips[rand];

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length;

        //destroy GameObject when the sound FX clip is done.
        Destroy(audioSource.gameObject, clipLength);
    }
}
