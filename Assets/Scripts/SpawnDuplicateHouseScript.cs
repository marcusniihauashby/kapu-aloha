using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDuplicateHouseScript : MonoBehaviour
{

    
    public GameObject houseDuplicate;
    // Start is called before the first frame update
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
            houseDuplicate.SetActive(true);
        }
    }
}
