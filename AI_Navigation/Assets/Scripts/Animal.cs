using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Enumeration to represent the states of the animal.
public enum AnimalState
{
    Idle, Moving
}

// Animal class responsible for handling the behavior of animals in the game.
[RequireComponent(typeof(NavMeshAgent))]
public class Animal : MonoBehaviour
{
    [Header("Wonder")]
    [SerializeField] private float wonderDistance = 50f;  // Maximum distance for wandering.
    [SerializeField] private float walkSpeed = 50f;       // Speed of the animal while walking.

    // To prevent the animal from getting stuck, limit the time it can walk to reach a destination.
    [SerializeField] private float maxWalkTime = 6f;

    [Header("Idle")]
    [SerializeField] private float idleTime = 5f;         // Time the animal stays idle.

    protected NavMeshAgent navMeshAgent;                  // Reference to the NavMeshAgent component.
    protected AnimalState currentState = AnimalState.Idle; // Current state of the animal.

    // Called when the script is first run.
    private void Start()
    {
        InitializeAnimal();
    }

    // Initialization method for the animal.
    protected virtual void InitializeAnimal()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = walkSpeed;

        currentState = AnimalState.Idle;
        UpdateState();
    }

    // Method to update the state of the animal based on the current state.
    protected virtual void UpdateState()
    {
        switch (currentState)
        {
            case AnimalState.Idle:
                HandleIdleState();
                break;
            case AnimalState.Moving:
                HandleMovingState();
                break;
        }
    }

    // Get a random valid NavMesh position within a specified distance from a given origin.
    protected Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;

        // Check if the random direction is a valid position on the NavMesh.
        if (NavMesh.SamplePosition(randomDirection, out var navMeshHit, distance, NavMesh.AllAreas))
        {
            return navMeshHit.position;
        }

        // If not valid, recursively try again.
        return GetRandomNavMeshPosition(origin, distance);
    }

    // Method to handle the Idle state of the animal.
    protected virtual void HandleIdleState()
    {
        StartCoroutine(WaitToMove());
    }

    // Coroutine to wait for a random amount of time before moving to a new destination.
    private IEnumerator WaitToMove()
    {
        var waitTime = Random.Range(idleTime / 2, idleTime * 2);
        yield return new WaitForSeconds(waitTime);

        var randomDestination = GetRandomNavMeshPosition(transform.position, wonderDistance);
        navMeshAgent.SetDestination(randomDestination);
        SetState(AnimalState.Moving);
    }

    // Method to handle the Moving state of the animal.
    protected virtual void HandleMovingState()
    {
        StartCoroutine(WaitToReachDestination());
    }

    // Coroutine to wait until the animal reaches its destination or exceeds the maximum walk time.
    private IEnumerator WaitToReachDestination()
    {
        float startTime = Time.time;
        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            // If the animal is moving for too long, reset its path and change state to Idle.
            if (Time.time - startTime > maxWalkTime)
            {
                navMeshAgent.ResetPath();
                SetState(AnimalState.Idle);
                yield break;
            }
            yield return null;
        }

        // If the animal has reached the destination, change state to Idle.
        SetState(AnimalState.Idle);
    }

    // Method to set the state of the animal.
    protected void SetState(AnimalState newState)
    {
        if (currentState == newState)
        {
            return;
        }
        currentState = newState;

        OnStateChanged(newState);
    }

    // Method called when the state of the animal changes.
    protected virtual void OnStateChanged(AnimalState newState)
    {
        UpdateState();
    }
}
