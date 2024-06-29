using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiDeathState : AiState
{
    public Vector3 direction;
    public AiStateId GetId()
    {
        return AiStateId.Death;
    }

    public void Enter(AiAgent agent)
    {
        agent.ragdoll.ActiveRagdoll();
        direction.y = 1;
        agent.ragdoll.ApplyForce(direction*agent.config.dieForce);
        agent.ui.gameObject.SetActive(false);
        agent.mesh.updateWhenOffscreen = true;
        agent.navMeshAgent.stoppingDistance = 0f;
        agent.navMeshAgent.destination = agent.transform.position;
        agent.navMeshAgent.speed = 0f;


    }

    public void Update(AiAgent agent)
    {
        
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
