using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerScript : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    // offset is a variable that is added to where audioclip is played. If you want to play the sound 10 meters south,
    // you put in the (-10, 0, 0) or some shit i have no idea how cardinal directions translate to fuckin unity
    [SerializeField] private Vector3 offset;
    public void OnTriggerEnter(Collider other)
    {
        if (audioClip != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(audioClip, transform.position + offset, 1f);
        }
        if (audioClip == null)
        {
            Debug.Log("You forgot to assign an audioclip to this SoundTrigger.");
        }
        Destroy(gameObject);
    }
}
