using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;

public class BoarLogic : MonoBehaviour
{

    // Player Variables
    
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public bool playerMadeNoise = false;
    public bool playerIsMoving = false;
    public bool playerInHearingRange = false;
    public float movementHearingThreshold = 2;

    // Boar Variables

    public NavMeshAgent agent;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    private bool canHear = true;
    private bool seesPlayer = false;
    public float secondsEntityCantHear = 2f;
    public float boarSpeed = 2f;
    public float reachThreshold = 1f;
    public float hearingRadius = 10f;
    public float chaseCooldown = 2f;

    private bool isWaitingToReturn = false;

    
    // HandlePatrol Variables
    public Vector3[] positions;
    private int indexMovingTowards = 0;
    private bool isMovingForwards = true;

    // HandleChase Variables
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //HandleSight Variables (add back later)
    // public bool playerInHearingRange, playerInAttackRange;

    public Vector3 lastHeardLocation;
    enum BoarState
    {
        Patrolling,
        Investigating,
        Chasing,
        PathingBack
    }

    private BoarState currentState = BoarState.Patrolling;

    /*

    noise logic, all footsteps have a location in which they were made.
    feed current location of player if player makes sound.

    */

    // Start is called before the first frame update


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Boar Update running. State: " + currentState);
        Debug.Log("Patrol Boolean: " + isMovingForwards);
        Debug.Log("IndexMovingTowards: " + indexMovingTowards + ", Positions.Length - 1: " + (positions.Length - 1));


        switch (currentState)
        {
            case BoarState.Patrolling:
                HandlePatrol();
                break;
            case BoarState.Investigating:
                HandleInvestigate();
                break;
            case BoarState.Chasing:
                HandleChase();
                break;
            case BoarState.PathingBack:
                HandlePathingBack();
                break;
        }

        HandleSight();
        HandleHearing();
    }

    void HandleSight()
    {
        if (seesPlayer)
        {
            currentState = BoarState.Chasing;
        }

    }

    void HandleHearing()
    {
        if (!canHear)
        {
            return;
        }

        // how do we define playerMadeNoise?
        // potential temporary/permanent solution
        // if !playerObject.isCrouching && playerIsMoving at that particular frame
        // Potential implementation below. Improvements are possible.

        CharacterController controller = playerObject.GetComponent<CharacterController>();

        playerIsMoving = controller.velocity.magnitude > movementHearingThreshold;

        playerMadeNoise = !playerObject.isCrouching && playerIsMoving;

        playerInHearingRange = Physics.CheckSphere(transform.position, hearingRadius, whatIsPlayer);

        bool heardPlayer = playerInHearingRange && playerMadeNoise;
        
        if (heardPlayer && currentState != BoarState.Chasing)
        {
            lastHeardLocation = playerObject.transform.position;
            currentState = BoarState.Investigating;
        }
    }

void HandlePatrol()
{
        if (positions == null || positions.Length == 0)
        {   
            Debug.Log("No positions found. Maybe you forgot to add positions into this boar.");
            return;
        }

    Vector3 target = positions[indexMovingTowards];

        // Move towards current target
        agent.SetDestination(positions[indexMovingTowards]);

    // Check if we've reached the target
    if (Vector3.Distance(transform.position, target) < reachThreshold)
    {
        // Change direction if we're at an endpoint
        if (isMovingForwards)
        {
            if (indexMovingTowards >= positions.Length - 1)
            {
                Debug.Log("Switching direction! Moving backwards now.");
                isMovingForwards = false;
                indexMovingTowards--;
            }
            else
            {
                indexMovingTowards++;
            }
        }
        else // moving backwards
        {
            if (indexMovingTowards <= 0)
            {
                isMovingForwards = true;
                indexMovingTowards++;
            }
            else
            {
                indexMovingTowards--;
            }
        }
    }
}

    void HandleInvestigate()
    {
        // path towards lastHeardLocation, look left and right
        agent.SetDestination(lastHeardLocation);
        if (Vector3.Distance(transform.position, lastHeardLocation) < reachThreshold)
        {
            if (!isWaitingToReturn)
            {
                StartCoroutine(WaitAndReturnToPatrol(2f));
            }
        }
    }

    IEnumerator WaitAndReturnToPatrol(float waitTime)
{
    Quaternion leftRotation = Quaternion.Euler(new Vector3 (0, transform.eulerAngles.y - 90f, 0));
    Quaternion rightRotation = Quaternion.Euler(new Vector3 (0, transform.eulerAngles.y + 90f, 0));

    float quarter = waitTime / 4;
    float half = waitTime / 2;
    float turnSpeed = 180f;

    isWaitingToReturn = true;

        //turn left, turn right
    
    yield return new WaitForSeconds(quarter);

    yield return new WaitForSeconds(quarter);

    yield return new WaitForSeconds(waitTime);
    isWaitingToReturn = false;

    currentState = BoarState.Patrolling;
}


    void HandleChase()
    {
        // set movement speed up, trigger sound cues + music

        // if seesPlayer, get current player location and path to it.

        // if !seesPlayer, path towards last seen player location
        // start coroutine towards changing state to BoarState.PathingBack after chaseCooldown secs
        Vector3 relativePosition = transform.position - playerObject.transform.position;
        if (seesPlayer)
        {
            // need a sound trigger
            agent.SetDestination(playerObject.transform.position);
            transform.rotation = Quaternion.LookRotation(relativePosition, new Vector3 (0, 1, 0));
        }
        if (!seesPlayer)
        {
            currentState = BoarState.Investigating;
        }

    }

    void HandlePathingBack()
    {
        // get to initial starting location

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
