using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    [SerializeField] private GameObject backgroundObject;
    private CanvasGroup backgroundObjectCanvasGroup;
    [SerializeField] private GameObject openingText;
    private CanvasGroup openingTextCanvasGroup;
    [SerializeField] private GameObject continueButton;
    private CanvasGroup continueButtonCanvasGroup;
    public const float TRANSITION_LENGTH = 1.5f;

    private bool canContinue = false;

    public void Start()
    {
        backgroundObjectCanvasGroup = backgroundObject.GetComponent<CanvasGroup>();
        openingTextCanvasGroup = openingText.GetComponent<CanvasGroup>();
        continueButtonCanvasGroup = continueButton.GetComponent<CanvasGroup>();
        backgroundObject.SetActive(false);
        openingText.SetActive(false);
        continueButton.SetActive(false);
    }

        public void Update()
    {
        // If the intro is finished AND the user clicks the primary mouse button (usually left-click)...
        if (canContinue && Input.GetMouseButtonDown(0))
        {
            ContinueToGame();
        }
    }
    public void PlayGame()
    {
        StartCoroutine(PlayOpeningSequence());
    }

    private IEnumerator PlayOpeningSequence()
    {
        // Activate the background and start its fade
        backgroundObject.SetActive(true);
        yield return StartCoroutine(FadeIn(backgroundObjectCanvasGroup, TRANSITION_LENGTH));

        yield return new WaitForSeconds(2f);

        // Wait for the background to finish, then activate and fade the text
        openingText.SetActive(true);
        yield return StartCoroutine(FadeIn(openingTextCanvasGroup, TRANSITION_LENGTH));

        yield return new WaitForSeconds(15f);

        // Wait for the text to finish, then activate and fade the continue button
        continueButton.SetActive(true);
        yield return StartCoroutine(FadeIn(continueButtonCanvasGroup, TRANSITION_LENGTH));

        canContinue = true;
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float elapsedTime = 0f;

        // Loop until the elapsed time is greater than the duration
        while (elapsedTime < duration)
        {
            // Increment the timer by the time since the last frame
            elapsedTime += Time.deltaTime;

            // Calculate the new alpha value
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the alpha is exactly 1 at the end
        canvasGroup.alpha = 1f;
    }

    public void ContinueToGame()
    {
        SceneManager.LoadScene(1);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}