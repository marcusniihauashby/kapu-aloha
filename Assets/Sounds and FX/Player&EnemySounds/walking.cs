using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walking : MonoBehaviour
{
    public AudioSource walkingSound, runningSound;

        void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            if (Input.GetKey(KeyCode.LeftShift))
            {
                walkingSound.enabled = false;
                runningSound.enabled = true;
            }
            else
            {
                walkingSound.enabled = true;
                runningSound.enabled = false;
            }
        }
        else
        {
            walkingSound.enabled = false;
            runningSound.enabled = false;
        }
    }
}
