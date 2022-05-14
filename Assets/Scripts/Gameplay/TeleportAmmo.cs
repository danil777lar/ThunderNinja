using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportAmmo : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    public Vector2 Normal { get; private set; }
    public Vector2 Velocity => _rigidbody.velocity;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        Normal = Vector2.up;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        Normal = collision.contacts[0].normal;
    }

    public void Shoot(Vector2 force) 
    {
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
        _rigidbody.angularVelocity = -1000;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
