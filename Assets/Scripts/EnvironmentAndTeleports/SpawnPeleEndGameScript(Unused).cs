using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnPeleEndGameScript : MonoBehaviour
{
    [SerializeField] private GameObject pele;
    [SerializeField] private GameObject finalConvoTrigger;
    public void OnTriggerEnter(Collider other)
    {
        pele.SetActive(true);
        finalConvoTrigger.SetActive(true);
    }
}
