using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public float interactRange = 3f;

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
            // Toggle active state
            isActive = !isActive;

            // Hint: don't disable the whole GameObject — just the visible/usable parts
            meshRenderer.enabled = isActive;

            // optionally also disable collider, sound, etc.
        }
    }
}

