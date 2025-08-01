using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarSpawningScript : MonoBehaviour
{
    [SerializeField] private GameObject boarToActivate;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (boarToActivate == null)
            {
                Debug.Log("You didn't add the baby boar to this collider. Connect the non-activated baby boar to this.");
                return;
            }
            boarToActivate.SetActive(true);
        }
    }
}
