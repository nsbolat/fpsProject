using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] _rigidbodies;
    private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactiveRagdoll();
    }

    public void DeactiveRagdoll()
    {
        foreach (var rigidBody in _rigidbodies)
        {
            rigidBody.isKinematic = true;
        }
        _animator.enabled = true;
    }
    
    public void ActiveRagdoll()
    {
        foreach (var rigidBody in _rigidbodies)
        {
            rigidBody.isKinematic = false;
        }
        _animator.enabled = false;
    }
    
    void Update()
    {
        
    }
}
