using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] private string[] _contactLayers;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider; 
    private Tween _gravityChangeTween;

    private bool _isGrounded = true;
    public bool IsGrounded 
    { 
        get => _isGrounded; 
        private set
        {
            if (!_isGrounded && value) 
            {
                AttachToGround();
                GroundStateChanged?.Invoke(value);
            }
            _isGrounded = value;
        } 
    }

    private bool _isCeiled;
    public bool IsCeiled 
    { 
        get => _isCeiled;
        private set 
        {
            if (!_isCeiled && value) 
            {
                AttachToCeil();
                CeilStateChanged?.Invoke(value);
            }
            _isCeiled = value;
        } 
    }

    private bool _isRightWallSlide;
    public bool IsRightWallSlide 
    { 
        get => _isRightWallSlide;
        private set 
        {
            if (!_isRightWallSlide && value)
            {
                AttachToWall();
                WallStateChanged?.Invoke(value);
            }
            _isRightWallSlide = value;
        } 
    }

    private bool _isLeftWallSlide;
    public bool IsLeftWallSlide 
    { 
        get => _isLeftWallSlide; 
        private set 
        {
            if (!_isLeftWallSlide && value)
            {
                AttachToWall();
                WallStateChanged?.Invoke(value);
            }
            _isLeftWallSlide = value;
        } 
    }

    public Vector2 Velocity => _rb.velocity;

    public Action AttachedToAnySurface;
    public Action<bool> GroundStateChanged;
    public Action<bool> CeilStateChanged;
    public Action<bool> WallStateChanged;



    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        IsGrounded = CastByDirection(Vector2.down);
        //IsCeiled = CastByDirection(Vector2.up) && !IsGrounded;
        IsRightWallSlide = CastByDirection(Vector2.right) && !IsGrounded && !IsCeiled && RaycastHeight(Vector2.right);
        IsLeftWallSlide = CastByDirection(Vector2.left) && !IsGrounded && !IsCeiled && !IsRightWallSlide && RaycastHeight(Vector2.left);

        if (!IsCeiled && !IsRightWallSlide && !IsLeftWallSlide) 
        {
            _gravityChangeTween?.Kill();
            _rb.gravityScale = 2f;
        }
    }

    public void ForceUpdate() 
    {
        FixedUpdate();
    }


    private bool CastByDirection(Vector2 direction) 
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask(_contactLayers);
        filter.useLayerMask = true;

        RaycastHit2D[] hits = new RaycastHit2D[1]; 
        return _rb.Cast(direction, filter, hits, 0.05f) > 0;
    }

    private bool RaycastHeight(Vector2 direction) 
    {
        int castCount = 20;
        int hits = 0;
        for (int i = 0; i < castCount; i++)
        {
            Vector3 origin = transform.position + (Vector3.up * Mathf.Lerp(0f, _collider.size.y, (float)i / (float)castCount));
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, _collider.size.x, LayerMask.GetMask(_contactLayers));
            if (hit && !hit.collider.OverlapPoint(origin))
            {
                hits++;
            }
        }

        return hits == castCount;
    }

    private void AttachToGround() 
    {
        AttachedToAnySurface?.Invoke();
        _gravityChangeTween?.Kill();
        _rb.gravityScale = 2f;
    }

    private void AttachToCeil() 
    {
        AttachedToAnySurface?.Invoke();
        _gravityChangeTween?.Kill();
        _rb.gravityScale = 0f;
        _rb.velocity = Vector2.zero;
        _gravityChangeTween = DOTween.To(() => 0f, (v) => { }, 0f, 0.25f)
            .OnComplete(() => _rb.gravityScale = 2f);
    }

    private void AttachToWall() 
    {
        AttachedToAnySurface?.Invoke();
        _gravityChangeTween?.Kill();
        _rb.gravityScale = 0f;
        _rb.velocity = Vector2.zero;
        _gravityChangeTween = DOTween.To(() => 0f, (v) => { }, 0f, 1f)
            .OnComplete(() => 
            {
                _gravityChangeTween = DOTween.To(() => 0f, (v) => _rb.gravityScale = v, 0.2f, 3f);
            });
    }
}
