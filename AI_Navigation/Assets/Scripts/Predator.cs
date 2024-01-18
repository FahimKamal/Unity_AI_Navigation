using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal
{
    [Header("Predator Variables")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float maxChaseTime = 10f;
    [SerializeField] private int biteDamage = 3;
    [SerializeField] private float biteCooldown = 2f;

    private Pray _currentChaseTarget;

    protected override void CheckChaseConditions()
    {
        if (_currentChaseTarget)
            return;

        var colliders = new Collider[10];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, detectionRange, colliders);

        for (int i = 0; i < numColliders; i++)
        {
            var pray = colliders[i].GetComponent<Pray>();
            if (pray != null)
            {
                StartChase(pray);
                return;
            }
        }

        _currentChaseTarget = null;
    }

    private void StartChase(Pray pray)
    {
        _currentChaseTarget = pray;
        SetState(AnimalState.Chase);
    }

    protected override void HandleChaseState()
    {
        if (_currentChaseTarget != null)
        {
            _currentChaseTarget.AlertPrey(this);

            StartCoroutine(ChasePray());
        }
        
        SetState(AnimalState.Idle);
    }

    private IEnumerator ChasePray()
    {
        var startTime = Time.time;
        while (_currentChaseTarget != null && Vector3.Distance(transform.position, _currentChaseTarget.transform.position) > navMeshAgent.stoppingDistance)
        {
            if (Time.time - startTime >= maxChaseTime || _currentChaseTarget == null)
            {
                StopChase();
                yield break;
            }
            SetState(AnimalState.Chase);
            navMeshAgent.SetDestination(_currentChaseTarget.transform.position);
            
            yield return null;
        }

        if (_currentChaseTarget)
        {
            _currentChaseTarget.ReceiveDamage(biteDamage);
        }
        yield return new WaitForSeconds(biteCooldown);
        _currentChaseTarget = null;
        HandleChaseState();
        
        CheckChaseConditions();
    }

    private void StopChase()
    {
        navMeshAgent.ResetPath();
        _currentChaseTarget = null;
        SetState(AnimalState.Idle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
