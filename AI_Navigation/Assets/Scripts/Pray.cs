using System;
using System.Collections;
using UnityEngine;

public class Pray : Animal
{
    [Header("Pray Variables")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float escapeMaxDistance = 80f;

    private Predator _currentPredator = null;

    public void AlertPrey(Predator predator)
    {
        SetState(AnimalState.Chase);
        _currentPredator = predator;
        StartCoroutine(RunFromPredator());
    }

    private IEnumerator RunFromPredator( )
    {
        // Wait until the predator is within detection range.
        while (_currentPredator == null || Vector3.Distance(transform.position, _currentPredator.transform.position) > detectionRange)
        {
            yield return null;
        }
        
        // Predator detected, so we should run away.
        while (_currentPredator != null || Vector3.Distance(transform.position, _currentPredator.transform.position) <= detectionRange)
        {
            RunAwayFromPredator();

            yield return null;
        }
        
        // Predator out of range, run to our final location and go back to idle. 
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        
        SetState(AnimalState.Idle);
    }

    private void RunAwayFromPredator()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                var runDirection = transform.position - _currentPredator.transform.position;
                var escapeDestination = transform.position + runDirection.normalized * (escapeMaxDistance * 2);
                navMeshAgent.SetDestination(GetRandomNavMeshPosition(escapeDestination, escapeMaxDistance));
                
            }
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
