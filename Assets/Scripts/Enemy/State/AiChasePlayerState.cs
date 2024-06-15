using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState //Alttakiler için aistate'e basıp alt + enter yaptım ve otomatik geldi.
{
    
    private float timer = 0.0f;

    
    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {

    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }
        
        timer -= Time.deltaTime;
        if (timer<0.0f)
        {
            Vector3 direction = (agent.playerTransform.position - agent.navMeshAgent.destination); 
            //float sQdistance = (playerTransform.position - agent.navMeshAgent.destination).magnitude;
            direction.y = 0;
            if (direction.sqrMagnitude>agent.config.maxDistance*agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.playerTransform.position;
                }
            }
            timer = agent.config.maxTime;
        }
    }

    public void Exit(AiAgent agent)
    {
        
    }
}
