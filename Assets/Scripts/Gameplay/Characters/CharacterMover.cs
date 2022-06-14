using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : CharacterPhysics
{
    [Space]
    [Header("Character Mover")]
    [SerializeField] private float _detectionDistance;

    public Action<Vector2> WallDetected;


    public void Move(Vector2 velocity) 
    {
        velocity.y = _rb.velocity.y;
        if (!IsGrounded && DetectWall(velocity.normalized))
        {
            velocity.x = 0f;
        }
        _rb.velocity = velocity;
    }

    private bool DetectWall(Vector2 direction) 
    {
        bool wallDetected = RaycastHeight(direction, 20, (_collider.size.x / 2f) + _detectionDistance, 0.05f) != 0;
        if (wallDetected)
            WallDetected?.Invoke(direction);
        return wallDetected;
    }
}
