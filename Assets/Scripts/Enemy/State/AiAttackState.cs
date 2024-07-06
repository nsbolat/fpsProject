using System.Collections;
using System.Collections.Generic;
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
        agent.animator.SetLayerWeight(2, 1f); // 2. katmanı etkinleştir
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
        agent.animator.SetLayerWeight(2, 0f); // 2. katmanı devre dışı bırak
        agent.navMeshAgent.isStopped = false; // Hareketi tekrar başlat

        // Animasyon eventi metod bağlantısını kaldır
        agent.OnAttackHit -= OnAttackHit;
    }

    private void Attack(AiAgent agent)
    {
        if (!agent.animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
        {
            int attackAnimation = Random.Range(0, 2); // 0 veya 1 rastgele animasyon seçimi
            agent.animator.SetBool("isAttacking", true);
        }
    }

    // Animasyon eventinden tetiklenecek metod
    private void OnAttackHit(AiAgent agent)
    {
        // Hasar verme işlemi
        agent.playerTransform.GetComponent<PlayerHealth>().TakePlayerDamage(5);
    }
}
