using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public float interactRange = 3f;

    public bool isFinalItem = false;
    private bool isActive = true;
    private MeshRenderer meshRenderer;

    void Start()
    {
        playerObject = GameObject.Find("FirstPersonController")
            .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, playerObject.transform.position);
        if (dist < interactRange && Input.GetKeyDown(KeyCode.E))
        {
            isActive = !isActive;
            meshRenderer.enabled = isActive;
            if (isFinalItem)
            {
                // teleport player to other location.
                /* 
                Get a vector displaying the distance from player to item.
                
                Flip player's y by 180. AND flip the vector 180 degrees
                Teleport player to item location + new flipped vector.
                */
                Vector3 offset = playerObject.transform.position - transform.position;
                Vector3 flippedOffset = Quaternion.Euler(0, 180, 0) * offset;

                Vector3 teleportLocation = GameObject.Find("Reference for Teleportation").transform.position;

                Vector3 newRotation = playerObject.transform.eulerAngles;

                var controller = playerObject.GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
                controller.RotateInstantly(180f);
                
                playerObject.transform.position = teleportLocation + flippedOffset;




            }
        }
    }
}

