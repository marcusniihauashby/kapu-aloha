using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BirdStoppingScript : MonoBehaviour
{
    // Start is called before the first frame update

    private MeshRenderer meshRenderer;
    [SerializeField] private GameObject birdAudio;
    private bool tookItem = false;

    [SerializeField] private GameObject dialogueTrigger;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        if (!tookItem && meshRenderer.enabled == false)
        {
            birdAudio.SetActive(false);
            tookItem = true;
            dialogueTrigger.SetActive(true);
        }
        
    }

}
