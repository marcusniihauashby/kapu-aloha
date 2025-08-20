using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyPeasyFirstPersonController;
using UnityEngine;

public class UIPrewarmer : MonoBehaviour
{
    // Drag your DialogueController's GameObject here in the Inspector
    [SerializeField] private GameObject dialogueCanvasObject;
    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    [SerializeField] private GameObject darkBackground;

    private CanvasGroup darkBackgroundCanvasGroup;

    void Start()
    {
        darkBackground.SetActive(true);
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();


        // Hardcoded value for turning player around. Dont worry about it.
        playerObject.RotateInstantly(180f);

        // This forces Unity to do all the expensive first-time setup now
        dialogueCanvasObject.SetActive(true);

        // Then we immediately hide it so the player doesn't see it
        dialogueCanvasObject.SetActive(false);

        darkBackgroundCanvasGroup = darkBackground.GetComponent<CanvasGroup>();

        StartCoroutine(FadeOut(darkBackgroundCanvasGroup, 2f));




    }

    IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float startTime = duration;

        // Loop until the elapsed time is greater than the duration
        while (startTime > 0f)
        {
            // Increment the timer by the time since the last frame
            startTime -= Time.deltaTime;

            // Calculate the new alpha value
            canvasGroup.alpha = Mathf.Clamp01(startTime / duration);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the alpha is exactly 1 at the end


        canvasGroup.alpha = 0f;
        // darkBackground.SetActive(false);
    }
}