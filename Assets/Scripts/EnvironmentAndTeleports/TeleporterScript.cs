using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isTeleporter;
    public bool teleporterActivated;

    private bool allItemsPutBack;
    public GameObject otherTeleporter;

    public GameObject[] itemsToTrack;

    public GameObject babyBoarToReset;
    public GameObject boarToReset;

    // reveal them one by one
    [SerializeField] private GameObject[] triggersToEnable;

    private int index = 0;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if this not a teleporter, return.
        if (!isTeleporter) return;

        // if this teleporter has not been activated, return.
        if (!teleporterActivated) return;

        // if this teleporter is activated, and this is a teleporter, then check items.
        foreach (GameObject item in itemsToTrack)
        {
            if (!item.GetComponent<MeshRenderer>().enabled)
            {
                // if there is an item that is still missing, then execute line below.
                allItemsPutBack = false;
                return;
            }
        }
        // if not, then all items have been put back.
        allItemsPutBack = true;
    }


    void OnTriggerEnter(Collider other)
    {
        // if this is not a teleporter, then return.
        if (!isTeleporter) return;

        // if this teleporter was not activated, then return.
        if (!teleporterActivated) return;

        // if this teleporter has all items put back, then return.
        if (allItemsPutBack) return;


        // if items are not put back, then teleport player.
        if (other.CompareTag("Player"))
        {
            // Get the root player object
            GameObject player = other.gameObject;

            // Find the truePosition child object
            Transform truePos = player.transform.Find("truePosition");
            if (truePos == null)
            {
                Debug.LogWarning("truePosition object not found on player.");
                return;
            }

            // Step 1: Get truePosition's offset relative to this teleporter
            Vector3 localOffset = transform.InverseTransformPoint(truePos.position);

            // Step 2: Convert that to world space using the other teleporter
            Vector3 targetTruePosWorld = otherTeleporter.transform.TransformPoint(localOffset);

            // Step 3: Offset the entire player so that truePosition ends up at the right spot
            Vector3 playerOffset = player.transform.position - truePos.position;
            player.transform.position = targetTruePosWorld + playerOffset;

            if (babyBoarToReset != null)
            {
                BabyBoarLogic babyBoarLogic = babyBoarToReset.GetComponent<BabyBoarLogic>();
                babyBoarLogic.indexMovingTowards = 0;
                babyBoarToReset.transform.position = babyBoarLogic.spawnPosition;
                babyBoarToReset.SetActive(false);
            }
            if (boarToReset != null)
            {
                BoarLogic boarLogic = boarToReset.GetComponent<BoarLogic>();
                boarLogic.indexMovingTowards = 0;
                boarToReset.transform.position = boarLogic.spawnPosition;
            }
            if (triggersToEnable != null && index < triggersToEnable.Length)
            {
                triggersToEnable[index].SetActive(true);
                index += 1;
            }

        }
    //     else if (other.CompareTag("Boar"))
    //     {
    //         GameObject mob = other.gameObject;
    //         Vector3 localOffset = transform.InverseTransformPoint(mob.transform.position);
    //         Vector3 targetTruePosWorld = otherTeleporter.transform.TransformPoint(localOffset);
    //         mob.transform.position = targetTruePosWorld;
    //         // change the 
    //         BoarLogic boarScript = mob.GetComponent<BoarLogic>();
    //         boarScript.indexMovingTowards = 0;
    //     }
    //     else if (other.CompareTag("Baby Boar"))
    //     {
    //         GameObject mob = other.gameObject;
    //         Vector3 localOffset = transform.InverseTransformPoint(mob.transform.position);
    //         Vector3 targetTruePosWorld = otherTeleporter.transform.TransformPoint(localOffset);
    //         mob.transform.position = targetTruePosWorld;
    //         BabyBoarLogic babyBoarLogic = mob.GetComponent<BabyBoarLogic>();
    //         babyBoarLogic.indexMovingTowards = 0;
    //     }
    }
}