using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotif : MonoBehaviour
{
    private NavMeshAgent agent;
    [Header("Player")] 
    
    [Space] 

    [Header("Distance")] 
    
    [Space] 

    [Space] 
    [Header("Animation")] 
    private Animator animator;
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.hasPath)
        {
            animator.SetFloat("Speed",agent.velocity.magnitude);

        }
        else
        {
            animator.SetFloat("Speed",0);
        }
    }
}
