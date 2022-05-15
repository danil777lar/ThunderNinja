using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControll : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private float _maxRange;
    [SerializeField] private TrajectoryDrawer _trajectory;
    [SerializeField] private TeleportAmmo _teleportAmmoPrefab;
    [SerializeField] private Transform _bodyHolder;
    [Header("Effects")]
    [SerializeField] private ParticleSystem _teleportInParticles;
    [SerializeField] private ParticleSystem _teleportOutParticles;

    private bool _pointerMoved;
    private PointerEventData _movePointerData;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private TeleportAmmo _teleportAmmoInstance;

    public Action PlayerTeleported;
    public Action ComputeAimEnd;
    public Action<Vector2> ComputeAim;
    public Action Destroyed;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

        PlayerControllSlideArea.Initialized += LinkSlideArea;
        if (PlayerControllSlideArea.Default) 
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
        PlayerControllSlideArea.Default.PointerDown -= OnPointerDown;
        PlayerControllSlideArea.Default.Drag -= OnPointerMove;
        PlayerControllSlideArea.Default.PointerUp -= OnPointerUp;
        Destroyed?.Invoke();
    }


    private void LinkSlideArea() 
    {
        PlayerControllSlideArea.Initialized -= LinkSlideArea;
        PlayerControllSlideArea.Default.PointerDown += OnPointerDown;
        PlayerControllSlideArea.Default.Drag += OnPointerMove;
        PlayerControllSlideArea.Default.PointerUp += OnPointerUp;
    }

    private void TryComputeAim() 
    {
        if (!_pointerMoved)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else 
        {
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _trajectory.ShowTrajectory(GetForce(_movePointerData.position));
            ComputeAim?.Invoke(_movePointerData.position);
        }
    }

    private void OnPointerDown(PointerEventData data) 
    {
        _pointerMoved = false;
        if (_teleportAmmoInstance) 
        {
            ParticleSystem partsIn = Instantiate(_teleportInParticles);
            ParticleSystem partsOut = Instantiate(_teleportOutParticles);
            partsIn.transform.position = transform.position + (Vector3.up * _collider.size.y / 2f);
            partsOut.transform.position = _teleportAmmoInstance.transform.position + (Vector3.up * _collider.size.y / 2f);
            partsIn.Play();
            partsOut.Play();
            Destroy(partsIn.gameObject, partsIn.main.duration);
            Destroy(partsOut.gameObject, partsOut.main.duration);

            transform.position = _teleportAmmoInstance.transform.position;
            partsOut.transform.SetParent(transform);
            FixPosition();
            _rigidbody.velocity = _teleportAmmoInstance.Velocity;

            _teleportAmmoInstance.Remove();
            _teleportAmmoInstance = null;

            _bodyHolder.localScale = Vector3.zero;
            _bodyHolder.DOScale(1f, 0.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(UpdateType.Normal, true);

            PlayerTeleported?.Invoke();
        }
    }

    private void OnPointerMove(PointerEventData data) 
    {
        _pointerMoved = true;
        _movePointerData = data;
    }

    private void OnPointerUp(PointerEventData data) 
    {
        _trajectory.HideTrajectory();
        if (_pointerMoved)
        {
            _teleportAmmoInstance = Instantiate(_teleportAmmoPrefab);
            _teleportAmmoInstance.transform.position = new Vector3(_trajectory.transform.position.x, _trajectory.transform.position.y, 0f);
            _teleportAmmoInstance.Shoot(GetForce(data.position));
            ComputeAimEnd?.Invoke();
        }
        _pointerMoved = false;
    }


    private Vector2 GetForce(Vector3 screenPosition) 
    {
        Vector2 direction = (screenPosition - Camera.main.WorldToScreenPoint((Vector2)_trajectory.transform.position)).normalized;
        return direction * _force;
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
