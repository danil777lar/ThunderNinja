using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterRagdoll : MonoBehaviour
{
    [SerializeField] private bool _2DMode;
    [SerializeField] private Rigidbody _root;

    private bool _isRagdollEnabled = true;
    private Animator _animator;
    private List<Rigidbody> _bones;


    private void Start()
    {
        _bones = new List<Rigidbody>(_root.GetComponentsInChildren<Rigidbody>());
        _animator = GetComponent<Animator>();
        DisableRagdoll();
    }

    private void FixedUpdate()
    {
        
    }

    public void EnableRagdoll(Vector3 velocity) 
    {
        if (_isRagdollEnabled) return;

        _isRagdollEnabled = true;
        _animator.enabled = false;
        foreach (Rigidbody rb in _bones)
        {
            rb.isKinematic = false;
            rb.velocity = velocity;
        }
    }

    public void DisableRagdoll()
    {
        if (!_isRagdollEnabled) return;

        _isRagdollEnabled = false;
        _animator.enabled = true;
        foreach (Rigidbody rb in _bones) 
        {
            rb.isKinematic = true;
        }
    }
}
