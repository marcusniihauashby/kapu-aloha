using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingScript : MonoBehaviour
{
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    private Renderer bushRenderer;
    private Coroutine dissolveCoroutine;
    private Material[] bushMaterials;

    // We need to know the original cutoff value to return to it.
    // 0.5 is the default for Unity's cutout shaders.
    private float originalCutoff = 0.5f;

    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
            .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();

        Transform snowBush = transform.Find("Snowbush");
        if (snowBush != null)
        {
            bushRenderer = snowBush.GetComponent<Renderer>();
            bushMaterials = bushRenderer.materials;
            
            // Optional: You can read the starting cutoff value if it's not 0.5
            if (bushMaterials.Length > 0)
            {
                originalCutoff = bushMaterials[0].GetFloat("_Cutoff");
            }
        }
        else
        {
            Debug.LogError("Child object 'Snowbush' not found!", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerObject.playerIsHiding = true;
            if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
            // Animate the cutoff to 1.0 to make the bush mostly disappear
            dissolveCoroutine = StartCoroutine(AnimateCutoff(1.0f, 1.0f));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerObject.playerIsHiding = false;
            if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
            // Animate the cutoff back to its original value
            dissolveCoroutine = StartCoroutine(AnimateCutoff(originalCutoff, 1.0f));
        }
    }

    /// <summary>
    /// Coroutine to animate the Alpha Cutoff of materials over a set duration.
    /// </summary>
    private IEnumerator AnimateCutoff(float targetCutoff, float duration)
    {
        if (bushRenderer == null) yield break;

        float elapsedTime = 0f;
        
        // Get the starting cutoff value from the first material
        float startCutoff = bushMaterials[0].GetFloat("_Cutoff");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the new cutoff value for this frame
            float newCutoff = Mathf.Lerp(startCutoff, targetCutoff, elapsedTime / duration);
            
            // Loop through each material and set the new cutoff value
            foreach (Material mat in bushMaterials)
            {
                // "_Cutoff" is the standard property name for Unity's shaders.
                mat.SetFloat("_Cutoff", newCutoff);
            }

            yield return null;
        }

        // After the loop, ensure the final value is set precisely
        foreach (Material mat in bushMaterials)
        {
            mat.SetFloat("_Cutoff", targetCutoff);
        }
    }
}