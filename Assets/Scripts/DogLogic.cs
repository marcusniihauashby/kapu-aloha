using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class DogLogic : MonoBehaviour
{

    // Player Variables

    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public bool playerMadeNoise = false;
    public bool playerIsMoving = false;
    public bool playerInHearingRange = false;
    public float movementHearingThreshold = 2f;

    // Dog Variables

    public Vector3 spawnLocation;

    public NavMeshAgent agent;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;
    private bool seesPlayer = false;
    public float runningMultiplier = 4f;
    public float reachThreshold = 1f;
    public float hearingRadius = 10f;
    public int mourningPosition = 999;

    private bool canHear = true;
    public float secondsEntityCantHear = 2f;



    // HandlePatrol Variables
    public Vector3[] positions;
    private int indexMovingTowards = 0;

    // HandleRunning Variables
    public Vector3 runningFinalLocation;
    private bool isRunning = false;
    private bool isFinishedRunning = false;
    private bool isMourning = false;
    private bool isFinishedMourning = false;


    //HandleSight Variables (add back later)
    // public bool playerInHearingRange, playerInAttackRange;
    enum DogState
    {
        Patrolling,
        Investigating,
        Running,
        Mourning
    }

    private DogState currentState = DogState.Patrolling;

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
        spawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case DogState.Patrolling:
                HandlePatrol();
                break;
            case DogState.Investigating:
                HandleInvestigate();
                break;
            case DogState.Running:
                HandleRunning();
                break;
            case DogState.Mourning:
                HandleMourning();
                break;
        }

        HandleSight();
        HandleHearing();
    }

    void HandleSight()
    {
        if (seesPlayer)
        {
            currentState = DogState.Running;
        }

    }

    void HandleHearing()
    {

        CharacterController controller = playerObject.GetComponent<CharacterController>();

        playerIsMoving = controller.velocity.magnitude > movementHearingThreshold;

        playerMadeNoise = !playerObject.isCrouching && playerIsMoving;

        playerInHearingRange = Physics.CheckSphere(transform.position, hearingRadius, whatIsPlayer);

        bool heardPlayer = playerInHearingRange && playerMadeNoise;

        if (heardPlayer)
        {
            currentState = DogState.Running;
        }
    }

    void HandlePatrol()
    {
        if (positions == null || positions.Length == 0)
        {
            Debug.Log("No positions found. Maybe you forgot to add positions into this dog.");
            return;
        }

        Vector3 target = positions[indexMovingTowards];

        // Move towards current target
        agent.SetDestination(target);

        // Check if we've reached the target
        if (Vector3.Distance(transform.position, target) < reachThreshold)
        {
            if (indexMovingTowards == mourningPosition - 1)
            {
                currentState = DogState.Mourning;
            }
            if (indexMovingTowards >= positions.Length - 1)
            {
                //TODO: Figure out where the dog is going when we finish.
                // iʻm thinking here of just making the dog teleport back to itʻs spawn location
                gameObject.transform.position = spawnLocation;
                indexMovingTowards = 0;

            }

            indexMovingTowards++;
        }
    }

    void HandleInvestigate()
    {
        // look at heardlocation, set coroutine to look at it for 5 seconds.
        // if you see someone for one contiguous second, then trigger seesPlayer as true.
        // if we finish, just continue patrolling.
    }

    void HandleRunning()
    {
        // look at player, look startled, make a noise, run away
        if (!isRunning) StartCoroutine(RunAway());
    }

    IEnumerator RunAway()
    {
        isRunning = true;
        Vector3 relativePos = playerObject.transform.position - transform.position;
        // transform.rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));
        agent.SetDestination(playerObject.transform.position);
        yield return new WaitForSeconds(0.1f);
        // make sound
        yield return new WaitForSeconds(1f);
        // run away coroutine, including destroying object
        agent.speed *= runningMultiplier;
        agent.SetDestination(runningFinalLocation);
        while (Vector3.Distance(transform.position, runningFinalLocation) > reachThreshold)
        {
            yield return null;
        }

        // hide the object
        isFinishedRunning = true;
        gameObject.SetActive(false);
        isRunning = false;
        // TODO: unhide and teleport dog back to starting location when player triggers teleporter.

    }

    private void HandleMourning() {
        if (!isMourning) StartCoroutine(Mourn());
    }

    IEnumerator Mourn() {
        isMourning = true;
        yield return new WaitForSeconds(5f);
        currentState = DogState.Patrolling;
        isMourning = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
