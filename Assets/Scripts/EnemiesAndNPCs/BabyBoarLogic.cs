using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BabyBoarLogic : MonoBehaviour
{

    // Player Variables
    private EasyPeasyFirstPersonController.FirstPersonController playerObject;
    public bool playerMadeNoise = false;
    public bool playerIsMoving = false;
    public bool playerInHearingRange = false;
    public float movementThreshold = 2f;
    public bool playerIsHiding = false;

    // Baby Boar Variables

    public Vector3 spawnPosition;

    public NavMeshAgent agent;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    public CapsuleCollider lineOfSight;
    public bool seesPlayer = false;
    public float secondsEntityCantHear = 2f; // unused
    public float boarSpeed = 2f;
    public float runningMultiplier = 4f;
    public float reachThreshold = 1f;
    public float hearingRadius = 30f;
    public Vector3 lastRecognizedPlayerLocation;


    // HandleSight Variables
    public bool playerInSightZone = false;
    private float playerVisibleTimer = 0f;
    public float requiredSightTime = 1.5f;
    public float raycastHeightOffset = 1.5f;
    public LayerMask sightObstructionMask; // Assign this in Inspector (should include "Obstacles" but exclude "Player")

    private bool playedInvestigateSound = false;

    // HandlePatrol Variables
    public Vector3[] positions;
    public int indexMovingTowards = 0;
    public int mourningPosition = 999;
    private bool isInvestigating = false;

    // HandleRunning Variables
    public Vector3 runningFinalLocation;
    private bool isRunning = false;
    private bool isMourning = false;

    enum BabyBoarState
    {
        Patrolling,
        Running,
        Mourning
    }

    private BabyBoarState currentState = BabyBoarState.Patrolling;
    public const float BOAR_WALK_VOLUME = 0.25f;

    public const float SOUND_EFFECT_VOLUME = 0.5f;

    public const float INVESTIGATE_SOUND_COOLDOWN = 5f;
    private AudioSource boarWalk;
    [SerializeField] private AudioClip boarHearsYou;
    [SerializeField] private AudioClip boarRunsAway;
    [SerializeField] private AudioClip boarMourns;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.Find("FirstPersonController")
        .GetComponent<EasyPeasyFirstPersonController.FirstPersonController>();
        spawnPosition = transform.position;
        boarWalk = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        playerIsHiding = playerObject.playerIsHiding;
        boarWalk.volume = (agent.velocity.magnitude > movementThreshold) ? BOAR_WALK_VOLUME : 0f;
        HandleSight();
        HandleHearing();
        switch (currentState)
        {
            case BabyBoarState.Patrolling:
                HandlePatrol();
                break;
            case BabyBoarState.Running:
                HandleRunning();
                break;
            case BabyBoarState.Mourning:
                HandleMourning();
                break;
        }

    }
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
        /* 
        If player is out of vision of the boar, then the boar ____.
        Enters investigation on the last seen location?
        
        */
        // Debug.Log(playerVisibleTimer + "is playerVisibleTimer");

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
                seesPlayer = true;
                if (!playedInvestigateSound)
                {
                    SoundFXManager.instance.PlaySoundFXClip(boarHearsYou, transform.position, SOUND_EFFECT_VOLUME);
                    StartCoroutine(InvestigateSoundCooldown(INVESTIGATE_SOUND_COOLDOWN));
                }
                if (playerVisibleTimer < requiredSightTime)
                {
                    playerVisibleTimer += Time.deltaTime;
                }

                if (playerVisibleTimer >= requiredSightTime)
                {
                    lastRecognizedPlayerLocation = playerObject.transform.position;
                    currentState = BabyBoarState.Running;
                }
                return;
            }
        }
        // if the raycast didn't trigger, the player is not visible
        seesPlayer = false;
    }


    IEnumerator InvestigateSoundCooldown(float seconds)
    {
        playedInvestigateSound = true;
        yield return new WaitForSeconds(seconds);
        playedInvestigateSound = false;
    }


    void HandleHearing()
    {

        CharacterController controller = playerObject.GetComponent<CharacterController>();

        playerIsMoving = controller.velocity.magnitude > movementThreshold;

        playerMadeNoise = !playerObject.isCrouching && playerIsMoving;

        playerInHearingRange = Physics.CheckSphere(transform.position, hearingRadius, whatIsPlayer);

        bool heardPlayer = playerInHearingRange && playerMadeNoise;

        if (heardPlayer && !isInvestigating && currentState != BabyBoarState.Running)
        {
            // start coroutine, look at player for 2-3 seconds.
            
            StartCoroutine(Investigate());
        }
    }
    IEnumerator Investigate()
    {
        isInvestigating = true;
        agent.isStopped = true;

        float timer = 0f;
        float duration = 2f;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            Vector3 direction = playerObject.transform.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            yield return null;
        }
        agent.isStopped = false;
        isInvestigating = false;
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
            if (indexMovingTowards == mourningPosition)
            {
                currentState = BabyBoarState.Mourning;
            }
            if (indexMovingTowards >= positions.Length - 1)
            {
                //TODO: Figure out where the dog is going when we finish.
                // iʻm thinking here of just making the dog teleport back to itʻs spawn location
                gameObject.transform.position = spawnPosition;
                indexMovingTowards = 0;

            }

            indexMovingTowards++;
        }
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
        SoundFXManager.instance.PlaySoundFXClip(boarRunsAway, transform.position, SOUND_EFFECT_VOLUME);
        while (Vector3.Distance(transform.position, runningFinalLocation) > reachThreshold)
        {
            yield return null;
        }

        currentState = BabyBoarState.Patrolling;
        indexMovingTowards = 0;
        
        // hides and teleports baby boar back to starting location when player triggers teleporter.
        agent.speed /= runningMultiplier;
        gameObject.SetActive(false);
        isRunning = false;
        gameObject.transform.position = spawnPosition;

    }

    private void HandleMourning()
    {
        if (!isMourning) StartCoroutine(Mourn());
    }

    IEnumerator Mourn()
    {
        SoundFXManager.instance.PlaySoundFXClip(boarMourns, transform.position, SOUND_EFFECT_VOLUME);
        isMourning = true;
        yield return new WaitForSeconds(5f);
        currentState = BabyBoarState.Patrolling;
        isMourning = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
