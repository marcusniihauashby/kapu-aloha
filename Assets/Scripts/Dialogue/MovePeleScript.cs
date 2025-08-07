using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovePeleScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    [SerializeField] private GameObject pele;

    [SerializeField] private Vector3 endLocation;
    public const float PELE_SPEED = 0.35f;

    private bool playedJumpscareSound = false;

    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // check rotation of player, put pele in front of the player ?
        if (!playedJumpscareSound)
        {
            audioSource.Play();
            playedJumpscareSound = true;
        }

        // lerp the person over to the thing
        StartCoroutine(MovePele(PELE_SPEED));
    }

    IEnumerator MovePele(float duration)
    {
        Debug.Log("Starting Coroutine");
        Vector3 startLocation = pele.transform.position;
        Vector3 desiredLocation = endLocation;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            // lerp all values from currentposition to desired position.
            float t = Mathf.Clamp01(timeElapsed / duration);
            Debug.Log("Pele Position" + pele.transform.position);
            pele.transform.position = Vector3.Lerp(startLocation, desiredLocation, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        pele.transform.position = desiredLocation;
        Debug.Log("Finished Coroutine");

        
    }
}
