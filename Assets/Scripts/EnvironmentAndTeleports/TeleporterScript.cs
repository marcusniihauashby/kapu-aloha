using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    // Start is called before the first frame update

    public bool isTeleporter;
    public GameObject otherTeleporter;

    public GameObject[] itemsToTrack;

    public GameObject babyBoarToReset;
    public GameObject boarToReset;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if the items in itemsToTrack are all set to active, turn off isTeleporter.
        // is this fine to check this if we're only having 3 items in itemsToTrack?
        if (!isTeleporter) return;
        foreach (GameObject item in itemsToTrack) {
            if (!item.GetComponent<MeshRenderer>().enabled)
            {
                return;
            }
        }
        isTeleporter = false;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!isTeleporter) return;

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