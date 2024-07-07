using System.Collections;
using UnityEngine;

public class AiAttackState : AiState
{
    private float attackCooldown = 1f; // Saldırılar arasındaki bekleme süresi
    private float lastAttackTime;
    private bool hasAttacked;

    public AiStateId GetId()
    {
        return AiStateId.Attack;
    }

    public void Enter(AiAgent agent)
    {
        lastAttackTime = Time.time - attackCooldown; // Saldırıyı hemen başlatmak için
        agent.StartCoroutine(IncreaseLayerWeight(agent, 2, 1f, 0.5f)); // Start the coroutine to increase layer weight over 0.5 seconds
        Attack(agent);
        hasAttacked = false;

        // Animasyon eventi dinlemek için metod bağlantısını kur
        agent.OnAttackHit += OnAttackHit;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }

        if (agent.playerTransform == null)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            return;
        }

        float distance = Vector3.Distance(agent.transform.position, agent.playerTransform.position);
        if (distance > agent.config.attackRange)
        {
            agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            return;
        }

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <=0)
        {
            agent.animator.SetInteger("AttackIndex", Random.Range(0, 4));
            attackCooldown = 1f;
        }
        
        if (Time.time >= lastAttackTime + attackCooldown && !hasAttacked)
        {
            Attack(agent);
            lastAttackTime = Time.time;
            hasAttacked = true;
        }

        // Saldırı animasyonu oynarken hareketi durdur (2. layer'ı kontrol et)
        if (agent.animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
        {
            agent.navMeshAgent.isStopped = true;
        }
        else
        {
            agent.navMeshAgent.isStopped = false;
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.animator.SetBool("isAttacking", false);
        agent.StartCoroutine(DecreaseLayerWeight(agent, 2, 0f, 0.5f)); // Start the coroutine to decrease layer weight over 0.5 seconds
        agent.navMeshAgent.isStopped = false; // Hareketi tekrar başlat

        // Animasyon eventi metod bağlantısını kaldır
        agent.OnAttackHit -= OnAttackHit;
    }

    private void Attack(AiAgent agent)
    {
        if (!agent.animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
        {
            agent.animator.SetInteger("AttackIndex", Random.Range(0, 4));
            agent.animator.SetBool("isAttacking", true);
        }
    }

    // Animasyon eventinden tetiklenecek metod
    private void OnAttackHit(AiAgent agent)
    {
        // Hasar verme işlemi
        agent.playerTransform.GetComponent<PlayerHealth>().TakePlayerDamage(5);
    }

    private IEnumerator IncreaseLayerWeight(AiAgent agent, int layerIndex, float targetWeight, float duration)
    {
        float startWeight = agent.animator.GetLayerWeight(layerIndex);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            agent.animator.SetLayerWeight(layerIndex, Mathf.Lerp(startWeight, targetWeight, time / duration));
            yield return null;
        }

        agent.animator.SetLayerWeight(layerIndex, targetWeight);
    }

    private IEnumerator DecreaseLayerWeight(AiAgent agent, int layerIndex, float targetWeight, float duration)
    {
        float startWeight = agent.animator.GetLayerWeight(layerIndex);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            agent.animator.SetLayerWeight(layerIndex, Mathf.Lerp(startWeight, targetWeight, time / duration));
            yield return null;
        }

        agent.animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
