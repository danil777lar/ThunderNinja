using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[RequireComponent(typeof(NamedAnimancerComponent))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Update Animations")]
    [SerializeField] private AnimationClip _idleAnim;
    [SerializeField] private AnimationClip _walkAnim;
    [SerializeField] private AnimationClip _runAnim;
    [SerializeField] private AnimationClip _fallAnim;
    [SerializeField] private AnimationClip _flyAnim;
    [SerializeField] private AnimationClip _wallHangAnim;
    [SerializeField] private AnimationClip _ceilHangAnim;
    [Header("Event Animations")]
    [SerializeField] private AnimationClip _landAnim;
    [Space]
    [SerializeField] private float _walkAnimSpeedScale;
    [SerializeField] private float _runAnimSpeedScale;
    [SerializeField] private Transform _armRoot;

    private bool _updateAnim = true;
    private float _ikWeight;
    private Vector3 _ikPosition;

    private NamedAnimancerComponent _animancer;
    private CharacterPhysics _physics;
    private PlayerJumpControll _jumpControll;
    private PlayerRunControll _runControll;
    private IEnumerator _updateAnimDelayCoroutine;


    private void Start()
    {
        _animancer = GetComponent<NamedAnimancerComponent>();
        _physics = GetComponentInParent<CharacterPhysics>();
        _jumpControll = GetComponentInParent<PlayerJumpControll>();
        _runControll = GetComponentInParent<PlayerRunControll>();

        _jumpControll.PlayerTeleported += OnPlayerTeleported;
        _physics.GroundStateChanged += OnGroundStateChanged;
        _physics.CeilStateChanged += OnCeilStateChanged;
        _physics.WallStateChanged += OnWallStateChanged;

        _animancer.Layers[0].ApplyAnimatorIK = true;
    }

    private void Update()
    {
        if (_updateAnim)
        {
            ComputeAnim(out AnimationClip anim, out float fade, out float speed);
            _animancer.Play(anim, fade).Speed = speed;
        }

        if (!_jumpControll.ComputeAim)
        {
            float targetRotation = transform.localRotation.eulerAngles.y;
            if (_physics.Velocity.x > 0f)
                targetRotation = 90f;
            else if (_physics.Velocity.x < 0f)
                targetRotation = -90f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.up * targetRotation), Time.deltaTime * 5f);

            _ikWeight = Mathf.Lerp(_ikWeight, 0f, Time.deltaTime * 15f / Time.timeScale);
        }
        else 
        {
            Vector2 position = (Vector2)_armRoot.position + _jumpControll.AimDirection.normalized * 10f;

            if (!_physics.IsLeftWallSlide && !_physics.IsRightWallSlide)
            {
                float targetRotation = transform.localRotation.eulerAngles.y;
                if (position.x > transform.position.x)
                    targetRotation = 90f;
                else if (position.x < transform.position.x)
                    targetRotation = -90f;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.up * targetRotation), Time.deltaTime * 15f / Time.timeScale);
            }

            _ikWeight = Mathf.Lerp(_ikWeight, 1f, Time.deltaTime * 15f / Time.timeScale);
            _ikPosition = new Vector3(position.x, position.y, -Camera.main.transform.position.z);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animancer.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
        _animancer.Animator.SetIKPosition(AvatarIKGoal.LeftHand, _ikPosition);
    }


    private void ComputeAnim(out AnimationClip anim, out float fade, out float speed)
    {
        anim = null;
        fade = 0.2f;
        speed = 1f;

        if (_physics.IsGrounded)
        {
            if (Mathf.Abs(_runControll.MoveSpeed.x) > 100f)
            {
                anim = _runAnim;
                speed = Mathf.Abs(_physics.Velocity.x) * _runAnimSpeedScale;
                fade = 0.5f;
            }
            else if (Mathf.Abs(_runControll.MoveSpeed.x) > 0f)
            {
                anim = _walkAnim;
                speed = Mathf.Abs(_runControll.MoveSpeed.x) * _walkAnimSpeedScale;
                fade = 0.5f;
            }
            else
            {
                anim = _idleAnim;
                fade = 0.5f;
            }
        }
        else if (_physics.IsLeftWallSlide)
        {
            anim = _wallHangAnim;
            transform.localRotation = Quaternion.Euler(Vector3.up * -90f);
        }
        else if (_physics.IsRightWallSlide)
        {
            anim = _wallHangAnim;
            transform.localRotation = Quaternion.Euler(Vector3.up * 90f);
        }
        else
        {
            if (Mathf.Abs(_physics.Velocity.x) > Mathf.Abs(_physics.Velocity.y) && Mathf.Abs(_physics.Velocity.x) > 5f)
                anim = _flyAnim;
            else
                anim = _fallAnim;
        }
    }

    private void PlayOnceAnim(AnimationClip clip, float fade) 
    {
        _animancer.Play(clip, fade);

        if (_updateAnimDelayCoroutine != null)
            StopCoroutine(_updateAnimDelayCoroutine);
        _updateAnimDelayCoroutine = UpdateAnimDelayCoroutine(clip.length);
        StartCoroutine(_updateAnimDelayCoroutine);
    }


    private void OnPlayerTeleported()
    {
        StartCoroutine(OnPlayerTeleportedCoroutine());
    }

    private void OnGroundStateChanged(bool arg) 
    {
        if (arg) 
        {
            if (Mathf.Abs(_physics.Velocity.x) <= 1f)
                PlayOnceAnim(_landAnim, 0.1f);
        }
    }

    private void OnCeilStateChanged(bool arg)
    {

    }

    private void OnWallStateChanged(bool arg)
    {

    }


    private IEnumerator OnPlayerTeleportedCoroutine() 
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        _physics.ForceUpdate();
        _animancer.Stop();
        ComputeAnim(out AnimationClip anim, out float fade, out float speed);
        _animancer.Play(anim);
    }

    private IEnumerator UpdateAnimDelayCoroutine(float delay) 
    {
        _updateAnim = false;
        yield return new WaitForSeconds(delay);
        _updateAnim = true;
    }
}
