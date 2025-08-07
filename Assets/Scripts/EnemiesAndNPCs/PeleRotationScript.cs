using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeleRotationScript : MonoBehaviour
{

    [SerializeField] private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    void Update()
    {
        if (playerObject != null)
        {
            Vector3 lookDirection = playerObject.transform.position - transform.position;
            lookDirection.y = 0; // This line is crucial to prevent the object from tilting up or down.

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = targetRotation;
        }
    }
}
