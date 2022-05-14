using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[RequireComponent(typeof(NamedAnimancerComponent))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Update Animations")]
    [SerializeField] private AnimationClip _idleAnim;
    [SerializeField] private AnimationClip _fallAnim;
    [SerializeField] private AnimationClip _flyAnim;
    [SerializeField] private AnimationClip _wallHangAnim;
    [SerializeField] private AnimationClip _ceilHangAnim;
    [Header("Event Animations")]
    [SerializeField] private AnimationClip _landAnim;
    [SerializeField] private AnimationClip _rollAnim;

    private bool _updateAnim = true;
    private bool _computeAim = false;
    private float _ikWeight;
    private Vector3 _ikPosition;

    private NamedAnimancerComponent _animancer;
    private PlayerPhysics _physics;
    private PlayerControll _controll;
    private IEnumerator _updateAnimDelayCoroutine;


    private void Start()
    {
        _animancer = GetComponent<NamedAnimancerComponent>();
        _physics = GetComponentInParent<PlayerPhysics>();
        _controll = GetComponentInParent<PlayerControll>();

        _controll.PlayerTeleported += OnPlayerTeleported;
        _controll.ComputeAim += OnComputeAim;
        _controll.ComputeAimEnd += OnComputeAimEnd;
        _physics.GroundStateChanged += OnGroundStateChanged;
        _physics.CeilStateChanged += OnCeilStateChanged;
        _physics.WallStateChanged += OnWallStateChanged;

        _animancer.Layers[0].ApplyAnimatorIK = true;
    }

    private void Update()
    {
        if (_updateAnim)
        {
            ComputeAnim(out AnimationClip anim, out float fade);
            _animancer.Play(anim, fade);
        }

        if (!_computeAim)
        {
            float targetRotation = transform.localRotation.eulerAngles.y;
            if (_physics.Velocity.x > 0f)
                targetRotation = 90f;
            else if (_physics.Velocity.x < 0f)
                targetRotation = -90f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.up * targetRotation), Time.deltaTime * 5f);

            _ikWeight = Mathf.Lerp(_ikWeight, 0f, Time.deltaTime * 15f / Time.timeScale);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        _animancer.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
        //_animancer.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);
        _animancer.Animator.SetIKPosition(AvatarIKGoal.LeftHand, _ikPosition);
    }


    private void ComputeAnim(out AnimationClip anim, out float fade)
    {
        anim = null;
        fade = 0.2f;

        if (_physics.IsGrounded)
        {
            if (Mathf.Abs(_physics.Velocity.x) > 1f)
                anim = _rollAnim;
            else 
                anim = _idleAnim;
        }
        else if (_physics.IsCeiled)
        {
            anim = _ceilHangAnim;
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

    private void OnComputeAim(Vector2 position) 
    {
        _computeAim = true;

        if (!_physics.IsLeftWallSlide && !_physics.IsRightWallSlide)
        {
            float targetRotation = transform.localRotation.eulerAngles.y;
            if (position.x > Camera.main.WorldToScreenPoint(transform.position).x)
                targetRotation = 90f;
            else if (position.x < Camera.main.WorldToScreenPoint(transform.position).x)
                targetRotation = -90f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.up * targetRotation), Time.deltaTime * 15f / Time.timeScale);
        }

        _ikWeight = Mathf.Lerp(_ikWeight, 1f, Time.deltaTime * 15f / Time.timeScale);
        _ikPosition = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, -Camera.main.transform.position.z));
    }

    private void OnComputeAimEnd()
    {
        _computeAim = false;
    }


    private IEnumerator OnPlayerTeleportedCoroutine() 
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        _physics.ForceUpdate();
        _animancer.Stop();
        ComputeAnim(out AnimationClip anim, out float fade);
        _animancer.Play(anim);
    }

    private IEnumerator UpdateAnimDelayCoroutine(float delay) 
    {
        _updateAnim = false;
        yield return new WaitForSeconds(delay);
        _updateAnim = true;
    }
}
