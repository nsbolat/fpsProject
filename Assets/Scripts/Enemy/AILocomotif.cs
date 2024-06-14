using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotif : MonoBehaviour
{
    private NavMeshAgent agent;
    [Header("Player")] public
        Transform playerTransform;
    
    [Space] 

    [Header("Distance")] 
    public float maxTime =1f, maxDistance = 1f;
    
    [Space] 

    [Space] 
    [Header("Animation")] 
    private Animator animator;
    private float timer = 0.0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer<0.0f)
        {
            float sQdistance = (playerTransform.position - agent.destination).magnitude;
            if (sQdistance>maxDistance*maxDistance)
            {
                agent.destination = playerTransform.position;
            }
            timer = maxTime;
        }
        animator.SetFloat("Speed",agent.velocity.magnitude);
    }
}
