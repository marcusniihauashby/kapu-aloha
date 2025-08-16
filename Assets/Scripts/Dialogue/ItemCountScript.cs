using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCountScript : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TMP_Text itemText;
    [SerializeField] private GameObject[] trackedItems;
    private int currentItemCount;
    [SerializeField] private float fadeDuration = 5f;

    [SerializeField] private string[] itemCountTexts = {
        "0 Items Left.",
        "1 Item Left.",
        "2 Items Left.",
        "3 Items Left.",
        "4 Items Left.",
        "5 Items Left.",
    };

    private int lowestItemCount;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemText = GetComponent<TMP_Text>();
        lowestItemCount = trackedItems.Length;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        Want to activate ONLY if we change the number for the first time.
        if number decrements, then display it.
        do not display anything until the number decrements lower than this one.
        */
        int activeItems = 0;
        foreach (GameObject item in trackedItems)
        {
            if (item.GetComponent<MeshRenderer>().enabled)
            {
                activeItems += 1;
            }
        }
        currentItemCount = activeItems;


        if (currentItemCount < lowestItemCount)
        {
            // Set new text to new value
            itemText.text = itemCountTexts[currentItemCount];

            // coroutine to lerp alpha up and then down again.
            StartCoroutine(FadeAlpha(fadeDuration));

            // set new lowestItemCount to prevent retriggers
            lowestItemCount = currentItemCount;
        }
    }

    IEnumerator FadeAlpha(float duration) {
        float timeElapsed = 0f;
        float halfDuration = duration / 2f;
        while (timeElapsed < halfDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timeElapsed / halfDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        timeElapsed = 0f;
        while (timeElapsed < halfDuration) {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timeElapsed / halfDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
