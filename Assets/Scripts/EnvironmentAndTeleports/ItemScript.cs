using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public float interactRange = 3f;

    public float maxCheckRange = 7f;

    public bool isFinalItem = false;
    private bool isActive = true;
    private bool isPuttingBack = false;
    private MeshRenderer meshRenderer;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private GameObject putDownText;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private GameObject finalDialogueTrigger;
    [SerializeField] private GameObject pele;

    private bool wastaken = false;

    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
            .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    // if we're close, we look at the meshrenderer. if meshrenderer active, we display pick up
    // if meshrenderer not active, we display 

    void Update()
    {
        float dist = Vector3.Distance(transform.position, playerObject.transform.position);
        if (dist < interactRange)
        {
            // display canvas saying press e to pick up/press e to put down
            if (meshRenderer.enabled)
            {
                putDownText.SetActive(false);
                pickUpText.SetActive(true);
            }
            else if (!meshRenderer.enabled)
            {
                pickUpText.SetActive(false);
                putDownText.SetActive(true);

            }
            if (Input.GetButtonDown("Action"))
            {
                if (!wastaken)
                {
                    // SHOW CANVAS OBJECT
                    // coroutine that takes 
                }
                wastaken = true;
                isActive = !isActive;
                meshRenderer.enabled = isActive;
                // SoundFXManager.instance.PlaySoundFXClip(audioClip, transform.position, 1f);
                if (isFinalItem)
                {
                    if (!isPuttingBack)
                    {
                        isPuttingBack = true;
                        Vector3 offset = playerObject.transform.position - transform.position;
                        Vector3 flippedOffset = Quaternion.Euler(0, 180, 0) * offset;

                        Vector3 teleportLocation = GameObject.Find("Reference for Teleportation").transform.position;

                        Vector3 newRotation = playerObject.transform.eulerAngles;

                        var controller = playerObject.GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
                        controller.RotateInstantly(180f);

                        playerObject.transform.position = teleportLocation + flippedOffset;
                    }
                    else
                    {
                        //activate final conversation trigger
                        finalDialogueTrigger.SetActive(true);
                        pele.SetActive(true);

                    }
                }
            }
        }
        else if (dist > interactRange && dist < maxCheckRange)
        {
            putDownText.SetActive(false);
            pickUpText.SetActive(false);
        }
    }
}

