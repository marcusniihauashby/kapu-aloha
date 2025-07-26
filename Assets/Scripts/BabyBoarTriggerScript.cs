using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyBoarTriggerScript : MonoBehaviour
{
    [SerializeField] private GameObject babyBoarToActivate;    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (babyBoarToActivate == null)
            {
                Debug.Log("You didn't add the baby boar to this collider. Connect the non-activated baby boar to this.");
                return;
            }
            babyBoarToActivate.SetActive(true);
        }
    }
}
