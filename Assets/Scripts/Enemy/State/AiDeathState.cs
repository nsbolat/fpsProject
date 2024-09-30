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
        agent.ui.gameObject.SetActive(false);
        agent.mesh.updateWhenOffscreen = true;
        agent.navMeshAgent.stoppingDistance = 0f;
        agent.navMeshAgent.destination = agent.transform.position;
        agent.navMeshAgent.speed = 0f;
        agent.playDeathSound();
        Debug.Log("is dead");


    }

    public void Update(AiAgent agent)
    {
        
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
