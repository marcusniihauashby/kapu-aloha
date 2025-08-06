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
using UnityEngine.UIElements;

public class BoarLogic : MonoBehaviour
{

    // Player Variables

    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public bool playerMadeNoise = false;
    public bool playerIsMoving = false;
    public bool playerInHearingRange = false;
    public float movementThreshold = 2;

    public bool playerIsHiding = false;

    // Boar Variables

    public NavMeshAgent agent;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;


    public Vector3 spawnPosition;
    private bool canHear = true;
    public bool seesPlayer = false;
    public float secondsEntityCantHear = 2f;
    public float boarSpeed = 2f;
    public float reachThreshold = 1f;
    public float hearingRadius = 10f;
    public float chaseCooldown = 2f;


    private bool isWaitingToReturn = false;


    // HandleSight Variables
    public CapsuleCollider lineOfSight;
    public bool playerInSightZone = false;
    private float playerVisibleTimer = 0f;
    public float requiredSightTime = 1.5f;
    public float raycastHeightOffset = 1.5f;
    public LayerMask sightObstructionMask; // Assign this in Inspector (should include "Obstacles" but exclude "Player")




    // HandlePatrol Variables
    public Vector3[] positions;
    public int indexMovingTowards = 0;
    private bool isMovingForwards = true;

    // HandleChase Variables
    public BoxCollider attackHitboxObject;
    private bool soundPlayed;
    private bool attackCoroutineStarted = false;
    private float attackHitboxSpawnDelay = 1.5f;



    // i had these for a reason that I no longer remember (add back later)
    // public bool playerInHearingRange, playerInAttackRange;

    public Vector3 lastRecognizedPlayerLocation;
    enum BoarState
    {
        Patrolling,
        Investigating,
        Chasing,
        PathingBack
    }

    private BoarState currentState = BoarState.Patrolling;
    public const float BOAR_WALK_VOLUME = 0.25f;
    public const float SOUND_EFFECT_VOLUME = 0.5f;
    private AudioSource boarWalk;
    [SerializeField] private AudioClip boarNoticesYou;
    [SerializeField] private AudioClip boarChaseStart;
    private bool playedNoticedPlayerSound = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
        lineOfSight = GetComponent<CapsuleCollider>();
        spawnPosition = transform.position;
        attackHitboxObject = GameObject.Find("AttackHitbox")
        .GetComponent<BoxCollider>();
        boarWalk = GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Boar Update running. State: " + currentState);
        // Debug.Log("IndexMovingTowards: " + indexMovingTowards + ", Positions.Length - 1: " + (positions.Length - 1));

        // update playerIsHiding
        playerIsHiding = playerObject.playerIsHiding;
        boarWalk.volume = (agent.velocity.magnitude > movementThreshold) ? BOAR_WALK_VOLUME : 0f;
        HandleSight();
        HandleHearing();
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
        }

    }
    // On the same GameObject with CapsuleCollider (Is Trigger checked)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSightZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSightZone = false;
        }
    }

    void HandleSight()
    {

        if (!playerInSightZone || playerIsHiding)
        {
            seesPlayer = false;
            if (playerVisibleTimer >= 0f)
            {
                playerVisibleTimer -= Time.deltaTime;
            }
            return;
        }
        Vector3 origin = transform.position + Vector3.up * raycastHeightOffset;
        Vector3 target = playerObject.transform.position + Vector3.up * raycastHeightOffset;
        Vector3 direction = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~sightObstructionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (seesPlayer == false)
                {
                    // TODO: implement seen by boar sound
                }
                if (!playedNoticedPlayerSound)
                {
                    playedNoticedPlayerSound = true;
                    SoundFXManager.instance.PlaySoundFXClip(boarNoticesYou, transform.position, SOUND_EFFECT_VOLUME);
                }
                seesPlayer = true;
                if (playerVisibleTimer < requiredSightTime)
                {
                    playerVisibleTimer += Time.deltaTime;
                }

                if (playerVisibleTimer >= requiredSightTime)
                {
                    lastRecognizedPlayerLocation = playerObject.transform.position;
                    currentState = BoarState.Chasing;
                }

                // else if (currentState != BoarState.Chasing)
                // {
                //     currentState = BoarState.Investigating;
                // }

                return;
            }
        }
        // if the raycast didn't trigger, the player is not visible
        seesPlayer = false;
        playedNoticedPlayerSound = false;
    }

    void HandleHearing()
    {
        if (!canHear)
        {
            return;
        }

        // Potential implementation below. Improvements are possible.
        // playerMadeNoise = if !playerObject.isCrouching && playerIsMoving at that particular frame

        CharacterController controller = playerObject.GetComponent<CharacterController>();

        playerIsMoving = controller.velocity.magnitude > movementThreshold;

        playerMadeNoise = !playerObject.isCrouching && playerIsMoving;

        playerInHearingRange = Physics.CheckSphere(transform.position, hearingRadius, whatIsPlayer);

        bool heardPlayer = playerInHearingRange && playerMadeNoise;

        if (heardPlayer && currentState != BoarState.Chasing)
        {
            lastRecognizedPlayerLocation = playerObject.transform.position;
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
                    indexMovingTowards = 0;
                    // isMovingForwards = false;
                    // indexMovingTowards--;
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
        // path towards lastRecognizedPlayerLocation, look left and right
        // TODO: Investigate sound trigger
        agent.SetDestination(lastRecognizedPlayerLocation);
        if (Vector3.Distance(transform.position, lastRecognizedPlayerLocation) < reachThreshold)
        {
            // if this triggers, we're at location and we look left and right. if we see someone, we break
            if (!isWaitingToReturn)
            {
                StartCoroutine(WaitAndReturnToPatrol(2f));
            }
        }
    }

    IEnumerator WaitAndReturnToPatrol(float waitTime)
    {
        if (!playedNoticedPlayerSound)
        {
            playedNoticedPlayerSound = true;
            SoundFXManager.instance.PlaySoundFXClip(boarNoticesYou, transform.position, SOUND_EFFECT_VOLUME);
        }
        Quaternion leftRotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y - 90f, 0));
        Quaternion rightRotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + 90f, 0));

        float quarter = waitTime / 4;
        float half = waitTime / 2;

        isWaitingToReturn = true;

        //turn left, turn right

        yield return new WaitForSeconds(quarter);

        yield return new WaitForSeconds(quarter);

        yield return new WaitForSeconds(waitTime);
        isWaitingToReturn = false;

        if (currentState != BoarState.Chasing)
        {
            playedNoticedPlayerSound = false;
            currentState = BoarState.Patrolling;
        }
    }


    void HandleChase()
    {
        // set movement speed up, trigger sound cues + music
        if (!soundPlayed)
        {
            soundPlayed = true;
            SoundFXManager.instance.PlaySoundFXClip(boarChaseStart, transform.position, SOUND_EFFECT_VOLUME);
        }
        Vector3 relativePosition = playerObject.transform.position - transform.position;
        if (seesPlayer)
        {
            // need a sound trigger
            agent.SetDestination(playerObject.transform.position);
            transform.rotation = Quaternion.LookRotation(relativePosition, new Vector3(0, 1, 0));
            if (!attackCoroutineStarted)
            {
                StartCoroutine(ActivateAttackHitboxAfterDelay(attackHitboxSpawnDelay));
                attackCoroutineStarted = true;
            }
        }

        if (!seesPlayer)
        {
            // de-aggro from sight over time
            playerVisibleTimer -= Time.deltaTime;
            if (playerVisibleTimer < 0f)
            {
                attackHitboxObject.enabled = false;
                currentState = BoarState.Investigating;
                soundPlayed = false;
                attackCoroutineStarted = false;
            }
        }

    }

    IEnumerator ActivateAttackHitboxAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        attackHitboxObject.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
