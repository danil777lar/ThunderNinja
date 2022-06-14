using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Larje.Core.Utils.WeaponControll;
using DG.Tweening;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private float _visionDistance = 1000f;
    [Space]
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private Image _shootImage;
    [SerializeField] private EnemyHitTrigger _hitTrigger;

    private bool _playerInVision;
    private bool _disableUpdate;
    private Transform _aimTarget;
    private PlayerJumpControll _playerControll;
    private BoxCollider2D _playerCollider;

    private Tween _shootTween;


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerControll = player.GetComponent<PlayerJumpControll>();
        _playerCollider = player.GetComponent<BoxCollider2D>();
        _playerControll.Destroyed += () => _disableUpdate = true;
        _hitTrigger.Killed += () =>
        {
            _disableUpdate = true;
            _shootTween?.Kill();
            _shootImage.fillAmount = 0f;
        };
    }

    private void Update()
    {
        if (_disableUpdate) return;

        if (_playerInVision)
            PlayerInVisionUpdate();
        else
            PlayerOutVisionUpdate();
    }


    private void PlayerInVisionUpdate()
    {
        if (!EnemyVisionTarget.Default.CheckIsInVision(transform, _visionDistance, 45f))
        {
            _playerInVision = false;
            _weapon.DisableWeapon();

            if (_aimTarget)
            {
                Destroy(_aimTarget.gameObject);
                _aimTarget = null;
                _weapon.SetAimTarget(null);
            }

            _shootImage.fillAmount = 0f;
            _shootTween?.Kill();

            return;
        }

        if (_aimTarget) 
        {
            _aimTarget.position = Vector3.Lerp(_aimTarget.position, _playerCollider.bounds.center, Time.deltaTime * 2.5f);
        }
    }

    private void PlayerOutVisionUpdate()
    {
        if (EnemyVisionTarget.Default.CheckIsInVision(transform, _visionDistance, 45f))
        {
            _playerInVision = true;
            _weapon.EnableWeapon();

            if (_aimTarget)
                Destroy(_aimTarget.gameObject);
            _aimTarget = new GameObject("Aim Target").transform;
            _aimTarget.SetParent(GameObject.FindGameObjectWithTag("Level").transform);
            _aimTarget.position = _playerCollider.bounds.center;
            _weapon.SetAimTarget(_aimTarget);

            _shootImage.fillAmount = 0f;
            _shootTween?.Kill();
            _shootTween = _shootImage.DOFillAmount(1f, 0.5f)
                .OnComplete(() =>
                {
                    _shootImage.fillAmount = 0f;
                    _weapon.Shoot();
                });

            return;
        }
    }
}
