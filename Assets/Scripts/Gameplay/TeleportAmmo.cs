using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportAmmo : MonoBehaviour
{
    [SerializeField] private float _throwLenght;
    [SerializeField] private ParticleSystem _shootParts;

    private bool _computePath;
    private float _passedLenght;
    private Rigidbody2D _rigidbody;

    public bool enableCollision = true;

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
                _shootParts?.Stop();
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
        if (!enableCollision)
            return;

        _computePath = false;
        StopAllCoroutines();

        _shootParts?.Stop();
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
        _shootParts?.Play();
    }

    public void Remove()
    {
        _shootParts.transform.SetParent(GameObject.FindGameObjectWithTag("Level").transform);
        Destroy(_shootParts.gameObject, _shootParts.main.duration);
        Destroy(gameObject);
    }

    public void SetActiveSimplePhysics(bool arg, float delay = 0f)
    {
        _computePath = false;

        if (arg)
        {
            StopAllCoroutines();
            _shootParts?.Play();
        }
        else 
        {
            StartCoroutine(FallAnim(delay));
        }
    }


    private IEnumerator FallAnim(float delay = 0f) 
    {
        yield return new WaitForSeconds(delay);
        _shootParts?.Stop();

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 0.2f;
        _rigidbody.drag = 1f;

        while (true)
        {
            _rigidbody.AddForce(Vector2.right * (Mathf.PerlinNoise(Time.time * 1f, 0f) - 0.5f) * 20f);
            _rigidbody.drag = Mathf.PerlinNoise(Time.time * 1f, 0f) * 3f;
            yield return new WaitForFixedUpdate();
        }
    }
}
