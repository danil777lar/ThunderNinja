using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportAmmo : MonoBehaviour
{
    [SerializeField] private float _throwLenght;

    private bool _computePath;
    private float _passedLenght;
    private Rigidbody2D _rigidbody;

    public Vector2 Normal { get; private set; }
    public Vector2 Velocity => _rigidbody.velocity;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        Normal = Vector2.up;
    }

    private void FixedUpdate()
    {
        if (_computePath)
        {
            _passedLenght += _rigidbody.velocity.magnitude * Time.fixedDeltaTime;
            if (_passedLenght >= _throwLenght) 
            {
                _computePath = false;
                StartCoroutine(FallAnim());        
            }
        }

        if (!_rigidbody.isKinematic) 
        {
            transform.right = Vector3.Lerp(transform.right, _rigidbody.velocity, Time.fixedDeltaTime * 10f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _computePath = false;
        StopAllCoroutines();

        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        transform.position = collision.contacts[0].point;
        Normal = collision.contacts[0].normal;
    }

    public void Shoot(Vector2 force) 
    {
        _computePath = true;
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }


    private IEnumerator FallAnim() 
    {
        _rigidbody.gravityScale = 0.2f;
        _rigidbody.drag = 10f;
        yield return new WaitForSeconds(0.5f);
        _rigidbody.drag = 1f;

        while (true)
        {
            _rigidbody.AddForce(Vector2.right * (Mathf.PerlinNoise(Time.time * 1f, 0f) - 0.5f) * 20f);
            _rigidbody.drag = Mathf.PerlinNoise(Time.time * 1f, 0f) * 3f;
            yield return new WaitForFixedUpdate();
        }
    }
}
