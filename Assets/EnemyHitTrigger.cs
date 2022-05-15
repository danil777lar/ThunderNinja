using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Services;

public class EnemyHitTrigger : MonoBehaviour
{
    //[SerializeField] private ParticleSystem _hitParts;
    [SerializeField] private CharacterRagdoll _ragdoll;

    private bool _isKilled;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isKilled) return;

        if (collision.GetComponent<PlayerControll>())
        {
                _isKilled = true;
                /*ParticleSystem partInstance = Instantiate(_hitParts);
                partInstance.transform.position = collision.transform.position;
                partInstance.transform.forward = rb.velocity;
                partInstance.Play();
                Destroy(partInstance.gameObject, partInstance.main.duration);*/

                _ragdoll.EnableRagdoll((transform.position - collision.transform.position).normalized * 5f);
        }
    }
}