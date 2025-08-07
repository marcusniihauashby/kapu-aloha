using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOutOfForestScript : MonoBehaviour
{

    [SerializeField] private GameObject dialogueController;
    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Vector3 finalDestination;

    public void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WaitForDialogueThenTeleport());
    }

    IEnumerator WaitForDialogueThenTeleport()
    {

        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => !dialogueController.gameObject.activeInHierarchy);

        playerObject.RotateInstantly(180f);
        playerObject.transform.position = finalDestination;
        SoundFXManager.instance.PlaySoundFXClip(audioClip, finalDestination, 1f);

        yield return new WaitForSeconds(12.5f);
        // TODO: Display End Credits.
    }
}
