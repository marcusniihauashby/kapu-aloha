using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interact_script : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject teleporter;
    public GameObject playerObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerObject.transform.position, transform.position) < 10f && Input.GetKeyDown(KeyCode.E))
        {
            TeleporterScript teleporterScript = teleporter.GetComponent<TeleporterScript>();
            teleporterScript.isTeleporter = false;

            Destroy(gameObject);
        }
    }
}
