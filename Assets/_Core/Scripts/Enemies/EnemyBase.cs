using _Core;
using Assets._Core.Scripts.Player;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour, IShootingTarget, IPoolableObject
{
    public event Action<EnemyBase> OnDeath;

    // Animator properties hashes
    private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    [SerializeField] private float initialHp;
    [SerializeField] private float corpseLifetime;
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotationSpeed;
    [Header("Attack")]
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float firstAttackDelay;
    [SerializeField] private float repeatedAttackDelay;

    private Transform targetTransform;
    
    private Animator _animator;
    private CapsuleCollider collider;
    private NavMeshAgent navAgent;

    private bool isFirstAttackMade;
    private float timer;

    public float HP { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsWalking => navAgent.enabled;
    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Transform target)
    {
        targetTransform = target;
        SetSpeed(walkSpeed);
        EnableWalking();
        EnableCollider();
        HP = initialHp;
        timer = 0;
        isFirstAttackMade = false;
        IsDead = false;
    }

    private void Update()
    {
        if (IsDead)
        {
            timer += Time.deltaTime;
            if (timer >= corpseLifetime)
            {
                Disable();
            }
            return;
        }
        if (IsWalking)
        {
            LookOnTarget();
            UpdateTargetPosition();
        }
        CheckIsAttacking();
        if (IsAttacking)
        {
            HandleAttack();
        }
    }

    private void HandleAttack()
    {
        timer += Time.deltaTime;
        if (!isFirstAttackMade && timer >= firstAttackDelay)
        {
            Attack();
            isFirstAttackMade = true;
        }
        if (isFirstAttackMade && timer >= repeatedAttackDelay)
        {
            Attack();
        }
    }

    private void CheckIsAttacking()
    {
        if (GetDistanceToTarget() <= attackDistance)
        {
            if (!IsAttacking)
            {
                StartAttacking();
            }
        }
        else if (IsAttacking)
        {
            StopAttacking();
        }
    }

    private void Attack()
    {
        if (targetTransform.TryGetComponent(out PlayerController player))
        {
            player.ReceiveDamage(attackDamage);
        }
        timer = 0;
    }

    private void LookOnTarget()
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void UpdateTargetPosition()
    {
        navAgent.SetDestination(targetTransform.position);
    }

    private void StartAttacking()
    {
        IsAttacking = true;
        //_animator.SetBool(IsAttackingHash, true);
        timer = 0;
        isFirstAttackMade = false;
        DisableWalking();
    }

    private void StopAttacking()
    {
        IsAttacking = false;
        //_animator.SetBool(IsAttackingHash, false);
        SetSpeed(walkSpeed);
        EnableWalking();
    }

    public void ReceiveDamage(float damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        //_animator.SetBool(IsDeadHash, true);
        DisableCollider();
        DisableWalking();
        StopAttacking();
        OnDeath?.Invoke(this);
        timer = 0;
    }

    private void EnableWalking()
    {
        navAgent.enabled = true;
        //_animator.SetBool(IsWalkingHash, true);
    }
    private void DisableWalking()
    {
        navAgent.enabled = false;
        //_animator.SetBool(IsWalkingHash, false);
    }

    private void EnableCollider()
    {
        collider.enabled = true;
    }

    private void DisableCollider()
    {
        collider.enabled = false;
    }

    private void SetSpeed(float speed)
    {
        navAgent.speed = speed;
    }

    private float GetDistanceToTarget()
    {
        return Vector3.Distance(transform.position, targetTransform.position);
    }

    public void SetTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }

    public void OnHit(Vector3 hitPoint, float damage, DamageType damageType)
    {
        ReceiveDamage(damage);
    }
}
