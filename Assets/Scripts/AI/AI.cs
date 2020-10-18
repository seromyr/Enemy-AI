using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Constants;
using System;

public class AI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public Transform[] waypoints;

    private int currentWaypointIndex;

    private Material material;
    private Color lOS_defaultColor, lOS_searchColor, lOS_alertColor1, lOS_alertColor2, lOS_invisibleColor;

    private string _name;

    [SerializeField]
    private AIState state;
    public AIState CurrentState { get { return state; } }
    private AIState previousState;
    public string Message { get; set; }
    private Queue<string> messages;
    private float mesTime;

    [SerializeField]
    private Transform target;
    private Vector3 targetPostion, lastKnownPosition;

    private float time, pursuitDuration, searchDuration, speedNormal, speedPursuit, speedFlee, attackRange, detectionRange;

    private int damage;

    private ParticleSystem powerBeam;

    private RaycastHit lineOfSight;

    [SerializeField]
    Transform hitTransform;

    [SerializeField]
    private float distanceToPlayer;

    private GameManager gameManager;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(waypoints[0].position);

        // Some long and unimportant setups
        {
            _name = gameObject.name;
            material = transform.Find("LineOfSight").GetComponent<MeshRenderer>().material;
            lOS_defaultColor = Color.green;
            lOS_defaultColor.a = 0.3f;
            material.color = lOS_defaultColor;

            lOS_searchColor = Color.yellow;
            lOS_searchColor.a = 0.3f;

            lOS_alertColor1 = Color.white;
            lOS_alertColor1.a = 0.3f;

            lOS_alertColor2 = Color.red;
            lOS_alertColor2.a = 0.3f;

            lOS_invisibleColor = Color.white;
            lOS_invisibleColor.a = 0;

            powerBeam = transform.Find("FX_PowerBeam").GetComponent<ParticleSystem>();
            powerBeam.Stop(true);

            messages = new Queue<string>();
        }

        // Default state
        state = AIState.Patrolling;

        // Set default message
        QueueThisMessageUp("Patrolling...");
        mesTime = Time.time;

        pursuitDuration = 5;
        searchDuration = 15;
        detectionRange = 8;
        attackRange = 5;
        damage = 1;

        // Navmesh agent speed
        speedNormal = 2f;
        speedPursuit = 5;
        speedFlee = 7;

        // Subscribe to event in GameManager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gameManager.OnPhaseChange += Flee;
    }

    void Update()
    {
        // AI performs routines
        AI_Routines();

        // AI pushes messages to screen
        PushMessages();
    }

    private void FixedUpdate()
    {
        LineOfSightUpdate();

        SituationEvaluate();
    }

    private void AI_Routines()
    {
        switch (state)
        {
            case AIState.Patrolling: Patrol(); break;
            case AIState.Chasing: Chase(); break;
            case AIState.Attacking: Attack(); break;
            case AIState.Searching: Search(); break;
            case AIState.Investigating: Investigate(); break;
            case AIState.Retreating: Retreat(); break;
            case AIState.Panic: Flee(); break;
        }
    }

    private void LineOfSightUpdate()
    {
        Vector3 rayStart = transform.position + transform.forward + Vector3.up; ;
        Vector3 rayEnd = transform.forward * detectionRange + transform.right * Mathf.Sin(Time.time * 15) * 5;

        Physics.Raycast(rayStart, rayEnd, out lineOfSight);
        Debug.DrawRay(rayStart, rayEnd, Color.red);

        // Debugging purpose
        hitTransform = lineOfSight.transform;
    }

    private void SituationEvaluate()
    {
        // AI sees player within its detection range in Normal situation
        if (state == AIState.Patrolling || state == AIState.Searching || state == AIState.Investigating || state == AIState.Retreating)
        {
            if (lineOfSight.transform != null && lineOfSight.transform.CompareTag("Player"))
            {
                distanceToPlayer = (transform.position - lineOfSight.transform.position).magnitude;
                if (distanceToPlayer <= detectionRange)
                {
                    target = lineOfSight.transform;
                    state = AIState.Chasing;
                    time = Time.time;

                    // Set messages
                    messages.Clear();
                    QueueThisMessageUp("Alert!");
                    QueueThisMessageUp("Intruder sighted!");
                    QueueThisMessageUp("Pursuing...");
                }
            }
            else if (lineOfSight.transform != null && lineOfSight.transform.CompareTag("SearchPoint"))
            {
                target = lineOfSight.transform;
            }
        }

        // AI sees player in emergency sistuation, with x2 detection range
        else if (state == AIState.Panic)
        {
            if (lineOfSight.transform != null && lineOfSight.transform.CompareTag("Player"))
            {
                distanceToPlayer = (transform.position - lineOfSight.transform.position).magnitude;
                if (distanceToPlayer <= detectionRange * 2)
                {
                    target = lineOfSight.transform;
                }
            }
        }

        // AI gets panicked when player obtained a relic
        if (gameManager.Phase == Phase.BeHunted && state != AIState.Panic)
        {
            state = AIState.Panic;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Oh my gosh!");
            QueueThisMessageUp("Run for your life!");
            QueueThisMessageUp("Doom has come to this world!");
        }

        // AI returns to Retreat state after panicking
        else if (gameManager.Phase == Phase.Hunt && state == AIState.Panic)
        {
            state = AIState.Retreating;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("What just happened?");
            QueueThisMessageUp("Lol lol idk..");
            QueueThisMessageUp("Back to position...");
        }
    }

    private void Patrol()
    {
        // AI patrol between waypoints (yellow collumns in the scene)
        material.color = lOS_defaultColor;
        gameObject.name = _name;
        navMeshAgent.speed = speedNormal;

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            // Incremental index and loop back to zero
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            //Debug.Log(currentWaypointIndex);
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Chase()
    {
        // AI stops shooting if it was
        powerBeam.Stop();

        // AI chases after player
        gameObject.name = "???";
        material.color = lOS_searchColor;
        targetPostion = target.position;

        // AI tries to catch up with player within a time frame
        if (Time.time < time + pursuitDuration)
        {
            navMeshAgent.SetDestination(targetPostion);
            navMeshAgent.speed = speedPursuit;

            // AI switches to attack mode if player gets inside attack range
            if (Mathf.Abs((transform.position - targetPostion).magnitude) <= attackRange)
            {
                state = AIState.Attacking;

                // Set messages
                messages.Clear();
                QueueThisMessageUp("Enemy is within range.");
                QueueThisMessageUp("Engaging...");
            }
        }

        // AI loses track of player if it cannot catch up with player after an amount of time
        else 
        {
            state = AIState.Searching;
            lastKnownPosition = target.position;
            navMeshAgent.SetDestination(lastKnownPosition);
            target = null;
            time = Time.time;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Target lost.");
            QueueThisMessageUp("Going to last know position...");
        }
    }

    private void Attack()
    {
        // AI stops to attack player from a distance
        targetPostion = target.position;
        navMeshAgent.speed = 0;

        // AI looks at player 
        Vector3 lookDirection = targetPostion - transform.position;
        lookDirection.y = 0;
        Quaternion lookAngle = Quaternion.LookRotation(lookDirection);
        transform.rotation = lookAngle;

        gameObject.name = "!!!!!";
        //material.color = Color.Lerp(lOS_alertColor1, lOS_alertColor2, Mathf.Sin(Time.time * 20));
        material.color = lOS_invisibleColor;

        // AI shoots and deals damage to player
        target.TryGetComponent<Player>(out var player);
        if (player != null)
        {
            powerBeam.Play(true);
            player.TakeDamage(damage);
        }

        // AI switches to pursuit mode if player moves out of its attack range
        if ((transform.position - targetPostion).magnitude > attackRange)
        {
            state = AIState.Chasing;
            time = Time.time;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Target is outside attack range.");
            QueueThisMessageUp("Pursuing...");
        }
    }

    private void Search()
    {
        // AI moves to player last known location 
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            // AI searches for player and a random search point by looking around
            transform.Rotate(Vector3.up, 180f * Time.deltaTime);

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Reached last known position");

            // AI investigates nearby locations if player is not found
            if (target != null && !target.CompareTag("Player"))
            {
                state = AIState.Investigating;
                time = Time.time;

                // Set messages
                QueueThisMessageUp("Investigating nearby locations.");
            }
        }


    }

    private void Investigate()
    {
        if (target == null)
        {
            // AI searches for player and a random search point by looking around
            transform.Rotate(Vector3.up, 180f * Time.deltaTime);
        }

        else
        {
            // AI goes to a random search point to look for player
            navMeshAgent.SetDestination(target.position);

            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                target = null;
            }
        }

        // AI gives up searching after an amount of time
        if (Time.time > time + searchDuration)
        {
            state = AIState.Retreating;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Unable to find intruder.");
            QueueThisMessageUp("Must have been the wind.");
            QueueThisMessageUp("Retreating...");
        }
    }

    private void Retreat()
    {
        // AI returns to the last waypoint and resumes patrolling
        gameObject.name = _name;
        navMeshAgent.speed = speedNormal;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);

        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            state = AIState.Patrolling;

            // Set messages
            messages.Clear();
            QueueThisMessageUp("Reached patrol checkpoint.");
            QueueThisMessageUp("Resuming patrol.");
            QueueThisMessageUp("Patrolling...");
        }
    }

    private void Flee()
    {
        gameObject.name = "T_T";

        // AI stops shooting if it was
        powerBeam.Stop();

        navMeshAgent.speed = speedFlee;
        
        // In case AI knows where player is
        if (target != null && target.CompareTag("Player"))
        {
            // Looks for a nearest Search point that far away from player and run towards it
            transform.Rotate(Vector3.up, 360f * Time.deltaTime);

            if (lineOfSight.transform != null
                && lineOfSight.transform.CompareTag("SearchPoint")
                && (lineOfSight.transform.position - target.position).magnitude >= (transform.position - target.position).magnitude * 2)
            {
                navMeshAgent.SetDestination(lineOfSight.transform.position);
            }
        }
        // In case AI does not know where player is
        else
        {
            transform.Rotate(Vector3.up, 360f * Time.deltaTime);

            // Just go randomly
            if (lineOfSight.transform != null && lineOfSight.transform.CompareTag("SearchPoint"))
            {
                navMeshAgent.SetDestination(lineOfSight.transform.position);
            }
        }
    }

    private void QueueThisMessageUp(string text)
    {
        if (!messages.Contains(text))
        {
            messages.Enqueue(text);
        }
    }

    private void PushMessages()
    {
        if (messages.Count > 0 && Time.time > mesTime + 0.5f)
        {
            Message = messages.Dequeue();
            mesTime = Time.time;
        }
    }
}
