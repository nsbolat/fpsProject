using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public Transform playerTransform;
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    public Ragdoll ragdoll;
    public SkinnedMeshRenderer mesh;
    public enemyUIHealthBar ui;
    public Animator animator;
    public AiSensor sensor;
    
    [Header("SOUNDS")]
        [Space(10)]
        public AudioClip[] footstepSounds; // Array of footstep sounds
        private AudioSource audioSource;
        public AudioClip attackSound;

        public AudioClip deathSound;
    
    
    public delegate void AttackHitEvent(AiAgent agent);
    public event AttackHitEvent OnAttackHit;

    private bool isAnimationPlaying;

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (audioSource==null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        ragdoll = GetComponent<Ragdoll>();
        animator = GetComponent<Animator>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        ui = GetComponentInChildren<enemyUIHealthBar>();
        sensor = GetComponent<AiSensor>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new AiStateMachine(this);

        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiDamage());
        stateMachine.RegisterState(new AiAttackState());

        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        isAnimationPlaying = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        navMeshAgent.isStopped = isAnimationPlaying;
    }
    
    private void AttackHit()
    {
        // Event tetiklenirse, abone olan tüm metodları çağır
        if (OnAttackHit != null)
        {
            OnAttackHit(this);
        }
    }
    
    public void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.pitch = 1f;
            AudioClip footstepClip = footstepSounds[randomIndex];
            audioSource.PlayOneShot(footstepClip);
        }
    }

    public void playDeathSound()
    {
        audioSource.pitch = 1f;
        audioSource.volume = 1f;
        audioSource.PlayOneShot(deathSound);
    }
}

