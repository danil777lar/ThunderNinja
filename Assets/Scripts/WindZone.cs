using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WindZone : MonoBehaviour
{
    [SerializeField] private bool _blockCollisions;
    [SerializeField] private float _power;
    [SerializeField] private int _partPerPower;
    [SerializeField] private ParticleSystem _parts;

    private BoxCollider2D _collider;


    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        ParticleSystem.VelocityOverLifetimeModule velocity = _parts.velocityOverLifetime;
        velocity.y = _power;
        velocity.orbitalY = _power;
        _parts.startLifetime = _collider.size.y / _power;

        ParticleSystem.EmissionModule emission = _parts.emission;
        emission.rateOverTime = _power * _partPerPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon")) 
        {
            TeleportAmmo ammo = collision.GetComponent<TeleportAmmo>();
            ammo.SetActiveSimplePhysics(true);
            if (_blockCollisions)
                ammo.enableCollision = false;

            collision.attachedRigidbody.velocity = Vector2.zero;
            collision.attachedRigidbody.drag = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Vector2 velocity = Vector2.zero;
            velocity += (Vector2)transform.up * _power;

            float distance = -(transform.InverseTransformPoint(collision.transform.position).x + (Mathf.Sin(Time.time * _power) * _collider.size.x / 2f));
            velocity += (Vector2)transform.right * _power * distance / (_collider.size.x / 2f);

            collision.attachedRigidbody.isKinematic = false;
            collision.attachedRigidbody.velocity = velocity;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            TeleportAmmo ammo = collision.GetComponent<TeleportAmmo>();
            ammo.SetActiveSimplePhysics(false, 5f);
            collision.attachedRigidbody.gravityScale = 0.5f;
            collision.attachedRigidbody.drag = 0f;
            if (_blockCollisions)
                ammo.enableCollision = true;
        }
    }
}
