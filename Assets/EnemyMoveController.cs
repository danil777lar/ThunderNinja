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

    private CharacterMover _mover;
    private Transform _targetPatrolBound;



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
        if (!(_usePatrol && _rightPatrolBound && _leftPatrolBound))
            return;

        if (!_targetPatrolBound) 
        {
            _targetPatrolBound = _rightPatrolBound;
            if (Vector2.Distance(_rightPatrolBound.position, transform.position) > Vector2.Distance(_leftPatrolBound.position, transform.position))
                _targetPatrolBound = _leftPatrolBound;
        }

        if (Vector2.Distance(transform.position, _targetPatrolBound.position) >= 0.1f)
        {
            _mover.Move((_targetPatrolBound.position - transform.position).normalized * _patrolSpeed);
        }
        else
        {
            _targetPatrolBound = (_targetPatrolBound == _rightPatrolBound ? _leftPatrolBound : _rightPatrolBound);
        }
    }

    private void OnWallDetected(Vector2 direction) 
    {
        _targetPatrolBound = (_targetPatrolBound == _rightPatrolBound ? _leftPatrolBound : _rightPatrolBound);
    }
}
