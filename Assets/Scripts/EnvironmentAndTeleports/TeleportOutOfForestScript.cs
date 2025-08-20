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

    [SerializeField] private TMP_Text creditsText1;
    [SerializeField] private TMP_Text creditsText2;
    [SerializeField] private TMP_Text creditsText3;

    [SerializeField] private float fadeDuration = 1f; // How long to fade in


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

        yield return new WaitForSeconds(7.5f);
        // TODO: Display End Credits.

        // fade alpha of black overlay to one
        yield return StartCoroutine(FadeAlphaIn(blackOverlay.GetComponent<CanvasGroup>(), fadeDuration));



        yield return StartCoroutine(FadeAlphaIn(creditsText1.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(9f);

        yield return StartCoroutine(FadeAlphaOut(creditsText1.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(FadeAlphaIn(creditsText2.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(9f);

        yield return StartCoroutine(FadeAlphaOut(creditsText2.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(FadeAlphaIn(creditsText3.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(9f);

        yield return StartCoroutine(FadeAlphaOut(creditsText3.GetComponent<CanvasGroup>(), fadeDuration));

        yield return new WaitForSeconds(1f);
    }

    IEnumerator FadeAlphaIn(CanvasGroup canvasGroup, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeAlphaOut(CanvasGroup canvasGroup, float duration) {
        float timeElapsed = 0f;
        while (timeElapsed < duration) {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
