using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAmmo : MonoBehaviour
{
    [SerializeField] private float _speedMin;
    [SerializeField] private float _speedMax;
    [SerializeField] ParticleSystem _pathParts;
    [SerializeField] ParticleSystem _hitParts;

    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = transform.forward * UnityEngine.Random.Range(_speedMin, _speedMax);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rb.isKinematic = true;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _pathParts.Stop();
        _hitParts.Play();
        Destroy(gameObject, _hitParts.main.duration);
    }
}
