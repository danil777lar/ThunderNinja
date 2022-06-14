using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Services;
using Larje.Core.Utils.WeaponControll;

public class EnemyHitTrigger : MonoBehaviour
{
    [SerializeField] private CharacterRagdoll _ragdoll;
    [SerializeField] private WeaponController _weapon;
    [SerializeField] private ParticleSystem _hitParts;
    [SerializeField] private ParticleSystem _bloodParts;

    private bool _isKilled;

    public Action Killed;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isKilled) return;

        if (collision.GetComponent<PlayerJumpControll>())
        {
            _isKilled = true;

            BoxCollider2D playerCollider = (BoxCollider2D)collision;
            BoxCollider2D selfCollider = GetComponent<BoxCollider2D>();

            ParticleSystem hitPartInstance = Instantiate(_hitParts);
            hitPartInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Level").transform);
            ParticleSystem bloodPartInstance = Instantiate(_bloodParts);
            bloodPartInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Level").transform);

            hitPartInstance.transform.position = playerCollider.bounds.center;
            hitPartInstance.transform.forward = -(hitPartInstance.transform.position - selfCollider.bounds.center);
            bloodPartInstance.transform.position = selfCollider.bounds.center;
            bloodPartInstance.transform.forward = -(hitPartInstance.transform.position - selfCollider.bounds.center);
            bloodPartInstance.Play();

            _weapon?.Drop();
            _ragdoll.EnableRagdoll((transform.position - collision.transform.position).normalized * 5f);

            Killed?.Invoke();
        }
    }
}