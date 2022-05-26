using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterPhysics))]
public class PlayerControll : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private float _loadTime;
    [SerializeField] private float _ammoDistance;
    [Space]
    [SerializeField] private float _minJoystickMagnitude;
    [Header("Links")]
    [SerializeField] private Transform _ammoSpawn;
    [SerializeField] private TrajectoryDrawer _trajectory;
    [SerializeField] private TeleportAmmo _teleportAmmoPrefab;
    [SerializeField] private Transform _bodyHolder;
    [Header("Effects")]
    [SerializeField] private ParticleSystem _teleportInParticles;
    [SerializeField] private ParticleSystem _teleportOutParticles;

    private float _loadTimePassed;

    private Rigidbody2D _rigidbody;
    private CharacterPhysics _physics;
    private BoxCollider2D _collider;
    private TeleportAmmo _teleportAmmoInstance;

    public bool ComputeAim { get; private set; }
    public Vector2 AimDirection { get; private set; }

    public Action PlayerTeleported;
    public Action Destroyed;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _physics = GetComponent<CharacterPhysics>();

        Joystick.Initialized += LinkSlideArea;
        if (Joystick.Default) 
        {
            LinkSlideArea();
        }
    }

    private void Update()
    {
        TryComputeAim();
    }

    private void OnDestroy()
    {
        Joystick.Default.PointerDown -= OnPointerDown;
        Joystick.Default.PointerUp -= OnPointerUp;
        Destroyed?.Invoke();
    }


    private void LinkSlideArea() 
    {
        Joystick.Initialized -= LinkSlideArea;
        Joystick.Default.PointerDown += OnPointerDown;
        Joystick.Default.PointerUp += OnPointerUp;
    }

    private void TryComputeAim() 
    {
        if (!Joystick.Default) return;

        if (Joystick.Default.Direction.magnitude < _minJoystickMagnitude)
        {
            _loadTimePassed = 0f;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _trajectory.HideTrajectory();
            ComputeAim = false;
        }
        else
        {
            _loadTimePassed += Time.deltaTime / Time.timeScale;
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _trajectory.maxLenght = _ammoDistance * Mathf.Min(_loadTimePassed / _loadTime, 1f);
            _trajectory.ShowTrajectory(GetForce(Joystick.Default.Direction));
            ComputeAim = true;
            AimDirection = GetForce(Joystick.Default.Direction);
        }
    }

    private void OnPointerDown() 
    {
        if (_teleportAmmoInstance) 
        {
            ParticleSystem partsIn = Instantiate(_teleportInParticles, GameObject.FindGameObjectWithTag("Level").transform, true);
            ParticleSystem partsOut = Instantiate(_teleportOutParticles, GameObject.FindGameObjectWithTag("Level").transform, true);
            partsIn.transform.position = transform.position + (Vector3.up * _collider.size.y / 2f);
            partsOut.transform.position = _teleportAmmoInstance.transform.position + (Vector3.up * _collider.size.y / 2f);
            partsIn.Play();
            partsOut.Play();
            Destroy(partsIn.gameObject, partsIn.main.duration);
            Destroy(partsOut.gameObject, partsOut.main.duration);

            transform.position = _teleportAmmoInstance.transform.position;
            partsOut.transform.SetParent(transform);
            FixPosition();
            _rigidbody.velocity = Vector2.up * 5f;

            _teleportAmmoInstance.Remove();
            _teleportAmmoInstance = null;

            _bodyHolder.localScale = Vector3.zero;
            _bodyHolder.DOScale(1f, 0.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(UpdateType.Normal, true);

            PlayerTeleported?.Invoke();
        }
    }

    private void OnPointerUp() 
    {
        _trajectory.HideTrajectory();
        if (Joystick.Default.Direction.magnitude >= _minJoystickMagnitude && _loadTimePassed >= _loadTime)
        {
            _teleportAmmoInstance = Instantiate(_teleportAmmoPrefab, GameObject.FindGameObjectWithTag("Level").transform, true);
            _teleportAmmoInstance.transform.position = new Vector3(_ammoSpawn.position.x, _ammoSpawn.position.y, 0f);
            _teleportAmmoInstance.Shoot(GetForce(Joystick.Default.Direction));
        }
    }


    private Vector2 GetForce(Vector3 direction) 
    {
        return -direction.normalized * _force * Mathf.Max(direction.magnitude, _minJoystickMagnitude);
    }

    private void FixPosition() 
    {
        int castCount = 100;
        for (int i = 0; i < castCount; i++)
        {
            Vector3 origin = transform.position + (Vector3.up * Mathf.Lerp(0f, _collider.size.y, (float)i / (float)castCount));
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.right, _collider.size.x, LayerMask.GetMask("Default"));
            if (hit && !hit.collider.OverlapPoint(origin))
            {
                Vector3 position = transform.position;
                position.x = hit.point.x - _collider.size.x / 2f;
                transform.position = position;
            }
        }

        for (int i = 0; i < castCount; i++)
        {
            Vector3 origin = transform.position + (Vector3.up * Mathf.Lerp(0f, _collider.size.y, (float)i / (float)castCount));
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.left, _collider.size.x, LayerMask.GetMask("Default"));
            if (hit && !hit.collider.OverlapPoint(origin))
            {
                Vector3 position = transform.position;
                position.x = hit.point.x + _collider.size.x / 2f;
                transform.position = position;
            }
        }

        for (int i = 0; i < castCount; i++)
        {
            Vector3 origin = transform.position - (Vector3.right * _collider.size.x / 2f) + (Vector3.right * Mathf.Lerp(0f, _collider.size.x, (float)i / (float)castCount));
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.up, _collider.size.y, LayerMask.GetMask("Default"));
            if (hit && !hit.collider.OverlapPoint(origin))
            {
                Vector3 position = transform.position;
                position.y = hit.point.y - _collider.size.y;
                transform.position = position;
            }
        }
    }
}
