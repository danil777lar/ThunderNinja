using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private bool _usePatrol;
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private float _patrolDelay;
    [SerializeField] private Transform _rightPatrolBound;
    [SerializeField] private Transform _leftPatrolBound;

    private bool _delayed;
    private CharacterMover _mover;
    private Transform _targetPatrolBound;

    public Vector2 MoveVelocity { get; private set; }



    private void Start()
    {
        _mover = GetComponent<CharacterMover>();
        _mover.WallDetected += OnWallDetected;
    }

    private void FixedUpdate()
    {
        TryComputePatrol();
    }

    private void TryComputePatrol() 
    {
        MoveVelocity = Vector2.zero;
        if (!_usePatrol || !_rightPatrolBound || !_leftPatrolBound || _delayed)
            return;

        if (!_targetPatrolBound) 
        {
            _targetPatrolBound = _rightPatrolBound;
            if (Vector2.Distance(_rightPatrolBound.position, transform.position) > Vector2.Distance(_leftPatrolBound.position, transform.position))
                _targetPatrolBound = _leftPatrolBound;
        }

        if (Vector2.Distance(transform.position, _targetPatrolBound.position) >= 0.1f)
        {
            MoveVelocity = (_targetPatrolBound.position - transform.position).normalized * _patrolSpeed;
            _mover.Move(MoveVelocity);
        }
        else
        {
            _targetPatrolBound = (_targetPatrolBound == _rightPatrolBound ? _leftPatrolBound : _rightPatrolBound);
            Delay();
        }
    }

    private void OnWallDetected(Vector2 direction) 
    {
        _targetPatrolBound = (_targetPatrolBound == _rightPatrolBound ? _leftPatrolBound : _rightPatrolBound);
        Delay();
    }

    private void Delay() 
    {
        StopAllCoroutines();
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine() 
    {
        _delayed = true;
        yield return new WaitForSeconds(_patrolDelay);
        _delayed = false;
    }
}
