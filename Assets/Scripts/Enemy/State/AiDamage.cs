using UnityEngine;

public class AiDamage : AiState
{
    private float damageTimer = 1.3f;
    private float transitionDuration = 0.5f;

    public AiStateId GetId()
    {
        return AiStateId.Damage;
    }

    public void Enter(AiAgent agent)
    {
        damageTimer = 0.5f;
        agent.navMeshAgent.speed = agent.config.defaultSpeed/2;
        agent.animator.SetLayerWeight(1, 1f); // Start with full weight
        agent.animator.SetTrigger("Damage");
    }

    public void Update(AiAgent agent)
    {
        damageTimer -= Time.deltaTime;
        
        // Calculate the interpolation parameter based on remaining damageTimer
        float t = Mathf.Clamp01(1f - (damageTimer / transitionDuration));
        
        // Smoothly transition from full weight (1) to zero weight (0) over time
        agent.animator.SetLayerWeight(1, Mathf.Lerp(1f, 0f, t));

        if (damageTimer <= 0f)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            agent.navMeshAgent.speed = agent.config.defaultSpeed;
            agent.animator.SetLayerWeight(1, Mathf.Lerp(1f, 0f, t));
        }
    }

    public void Exit(AiAgent agent)
    {
        // Clean up any necessary states upon exiting
    }
}