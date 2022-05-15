using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Larje.Core.Services;

public class PlayerHitTrigger : MonoBehaviour
{
    [InjectService] private UIService _uiService;

    [SerializeField] private ParticleSystem _hitParts;
    [SerializeField] private CharacterRagdoll _ragdoll;

    private bool _isKilled;


    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ammo")) 
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb) 
            {
                ParticleSystem partInstance = Instantiate(_hitParts);
                partInstance.transform.position = collision.transform.position;
                partInstance.transform.forward = rb.velocity;
                partInstance.Play();
                Destroy(partInstance.gameObject, partInstance.main.duration);

                _ragdoll.EnableRagdoll(rb.velocity.normalized * 5f);
                StartCoroutine(KillPlayerCoroutine());
            }
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }


    private IEnumerator KillPlayerCoroutine() 
    {
        if (!_isKilled)
        {
            _isKilled = true;

            Time.timeScale = 0.025f;
            Time.fixedDeltaTime = 0.02f * 0.025f;

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            _ragdoll.transform.SetParent(transform.parent.parent);
            _ragdoll.GetComponent<PlayerAnimation>().enabled = false;

            GameObject playerBase = transform.parent.gameObject;
            transform.SetParent(_ragdoll.transform.parent);
            GetComponent<Collider2D>().enabled = false;
            Destroy(playerBase);

            yield return new WaitForSecondsRealtime(1f);

            _uiService.ShowScreen("Fail");
        }
    }
}
