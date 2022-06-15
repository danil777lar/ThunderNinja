using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

[RequireComponent(typeof(NamedAnimancerComponent))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip _idleAnim;
    [SerializeField] private AnimationClip _walkAnim;
    [SerializeField] private AnimationClip _runAnim;
    [Space]
    [SerializeField] private float _walkAnimSpeedScale;

    private NamedAnimancerComponent _animancer;
    private EnemyMoveControll _moveController;


    private void Start()
    {
        _animancer = GetComponent<NamedAnimancerComponent>();
        _moveController = GetComponentInParent<EnemyMoveControll>();

        _animancer.Layers[0].ApplyAnimatorIK = true;
    }

    private void Update()
    {
        ComputeAnim(out AnimationClip anim, out float fade, out float speed);
        _animancer.Play(anim, fade).Speed = speed;
    }


    private void ComputeAnim(out AnimationClip anim, out float fade, out float speed) 
    {
        anim = _idleAnim;
        fade = 0.25f;
        speed = 1f;

        if (_moveController.MoveVelocity != Vector2.zero) 
        {
            anim = _walkAnim;
            speed = Mathf.Abs(_moveController.MoveVelocity.x) * _walkAnimSpeedScale;

            Quaternion targetRotation = Quaternion.Euler(Vector3.up * (_moveController.MoveVelocity.x > 0f ? 90f : -90f));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
