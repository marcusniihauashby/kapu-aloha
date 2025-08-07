using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HouseEnterAndExitScript : MonoBehaviour
{

    public bool isDuplicateHouse = false;
    public GameObject houseDuplicate;
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;

    private Coroutine angleCheckCoroutine;

    public GameObject[] teleporters;

    public GameObject[] triggers;
    private bool teleportersActivated;

    [SerializeField] private GameObject loopTwoDialogueTriggersContainer;
    [SerializeField] private GameObject loopTwoSoundTriggersContainer;



    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (isDuplicateHouse && other.CompareTag("Player"))
        {
            loopTwoDialogueTriggersContainer.SetActive(true);
            loopTwoSoundTriggersContainer.SetActive(true);
            if (!teleportersActivated)
            {
                foreach (GameObject teleporter in teleporters)
                {
                    TeleporterScript teleporterScript = teleporter.GetComponent<TeleporterScript>();
                    teleporterScript.isTeleporter = true;
                }
                teleportersActivated = true;

                foreach (GameObject trigger in triggers)
                {
                    trigger.SetActive(true);
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isDuplicateHouse)
        {
            if (other.CompareTag("Player"))
            {
                houseDuplicate.SetActive(true);
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
        if (isDuplicateHouse)
        {
            // if player's y rotation is ever between 140 and 220, houseDuplicate.SetActive(false);
            if (other.CompareTag("Player"))
            {
                if (angleCheckCoroutine == null)
                {
                    angleCheckCoroutine = StartCoroutine(CheckPlayerRotationCoroutine());
                }
            }
        }
    }
    IEnumerator CheckPlayerRotationCoroutine()
    {
        while (true)
        {
            float yRot = playerObject.transform.eulerAngles.y;

            if (yRot > 140f && yRot < 220f)
            {
                houseDuplicate.SetActive(false);
                break; // exit the loop and end the coroutine
            }

            yield return null; // wait 1 frame
        }

        angleCheckCoroutine = null; // clean up reference
    }
}

