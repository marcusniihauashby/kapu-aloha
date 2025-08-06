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

    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText dialogueText;

    [SerializeField] private AudioClip jumpscareSound;

    public const float JUMPSCARE_VOLUME = 0.5f;

    private bool playedJumpscareSound = false;

    private bool handledConversation = false;




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
            }
        }
        if (isDuplicateHouse)
        {
            // TODO: Have pele move in front of player
            if (!playedJumpscareSound)
            {
                SoundFXManager.instance.PlaySoundFXClip(jumpscareSound, transform.position, JUMPSCARE_VOLUME);
                playedJumpscareSound = true;
            }
            if (!handledConversation)
            {
                dialogueController.HandleConversation(dialogueText);
                handledConversation = true;
            }
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

