using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : CharacterPhysics
{
    [Space, Space]
    [Header("Character Mover")]
    [SerializeField] private float _detectionDistance;

    public Action<Vector2> WallDetected;


    public void Move(Vector2 velocity) 
    {
        if (IsGrounded && !DetectWall(velocity.normalized))
        {
            velocity.y = 0f;
            _rb.velocity = velocity;
        }
        else 
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private bool DetectWall(Vector2 direction) 
    {
        bool wallDetected = RaycastHeight(direction, 20, (_collider.size.x / 2f) + _detectionDistance) != 0;
        WallDetected?.Invoke(direction);
        return wallDetected;
    }
}
