using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

namespace Larje.Core.Utils.WeaponControll
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private NamedAnimancerComponent _animancer;

        private float _ikWeight;
        private Weapon _weapon;


        private void Start()
        {
            _weapon = GetComponentInChildren<Weapon>();

            _animancer.Layers[0].ApplyAnimatorIK = true;
        }


        private void OnAnimatorIK(int layerIndex)
        {
            _ikWeight = 1f;

            if (_weapon && _weapon.RightArmTarget) 
            {
                _animancer.Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _ikWeight);
                _animancer.Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _ikWeight);
                _animancer.Animator.SetIKPosition(AvatarIKGoal.RightHand, _weapon.RightArmTarget.position);
                _animancer.Animator.SetIKRotation(AvatarIKGoal.RightHand, _weapon.RightArmTarget.rotation);
            }
            if (_weapon && _weapon.LeftArmTarget) 
            {
                _animancer.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _ikWeight);
                _animancer.Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _ikWeight);
                _animancer.Animator.SetIKPosition(AvatarIKGoal.LeftHand, _weapon.LeftArmTarget.position);
                _animancer.Animator.SetIKRotation(AvatarIKGoal.LeftHand, _weapon.LeftArmTarget.rotation);
            }
        }

        [ContextMenu("Enable Weapon")]
        private void EnableWeapon() 
        {
            _weapon?.ChangeState(true, 0.5f);
        }

        [ContextMenu("Disable Weapon")]
        private void DisableWeapon()
        {
            _weapon?.ChangeState(false, 1f);
        }

        [ContextMenu("Shoot")]
        private void Shoot() 
        {
            _weapon?.Shoot();
        }

        public void Drop() 
        {
            _weapon?.Drop();
            _weapon = null;
        }
    }
}