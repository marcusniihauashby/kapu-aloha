using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainStartScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject rainToActivate;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rainToActivate.SetActive(true);
            Destroy(gameObject);
            // TODO: TURN ON RAIN SOUNDS
        }
    }
}
