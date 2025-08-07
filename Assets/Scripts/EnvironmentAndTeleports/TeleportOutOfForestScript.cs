using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleportOutOfForestScript : MonoBehaviour
{

    [SerializeField] private GameObject dialogueController;
    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Vector3 finalDestination;

    [SerializeField] private GameObject blackOverlay;

    [SerializeField] private TMP_Text creditsText;

    [SerializeField] private float fadeDuration = 2f; // How long to fade in


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

        blackOverlay.SetActive(true);
        yield return new WaitForSeconds(1f);
        creditsText.gameObject.SetActive(true);
        // Reset alpha to 0 before fading
        Color textColor = creditsText.color;
        textColor.a = 0;
        creditsText.color = textColor;

        yield return StartCoroutine(FadeInText());

    }
        IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        Color c = creditsText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            creditsText.color = c;
            yield return null;
        }
    }

}
