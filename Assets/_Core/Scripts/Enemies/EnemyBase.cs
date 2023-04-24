using _Core;
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

    [SerializeField] private float _initialHp;
    [SerializeField] private float _corpseLifetime;
    [Header("Movement")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _rotationSpeed;
    [Header("Attack")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _firstAttackDelay;
    [SerializeField] private float _repeatedAttackDelay;

    private Transform _targetTransform;
    
    private Animator _animator;
    private CapsuleCollider _collider;
    private NavMeshAgent _navAgent;

    private bool _isFirstAttackMade;
    private float _timer;

    public float HP { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsWalking => _navAgent.enabled;
    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _navAgent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Transform target)
    {
        _targetTransform = target;
        SetSpeed(_walkSpeed);
        EnableWalking();
        EnableCollider();
        HP = _initialHp;
        _timer = 0;
        _isFirstAttackMade = false;
        IsDead = false;
    }

    private void Update()
    {
        if (IsDead)
        {
            _timer += Time.deltaTime;
            if (_timer >= _corpseLifetime)
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
        _timer += Time.deltaTime;
        if (!_isFirstAttackMade && _timer >= _firstAttackDelay)
        {
            Attack();
            _isFirstAttackMade = true;
        }
        if (_isFirstAttackMade && _timer >= _repeatedAttackDelay)
        {
            Attack();
        }
    }

    private void CheckIsAttacking()
    {
        if (GetDistanceToTarget() <= _attackDistance)
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
        if (_targetTransform.TryGetComponent(out PlayerController player))
        {
            player.ReceiveDamage(_attackDamage);
        }
        _timer = 0;
    }

    private void LookOnTarget()
    {
        Vector3 direction = (_targetTransform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
    }

    private void UpdateTargetPosition()
    {
        _navAgent.SetDestination(_targetTransform.position);
    }

    private void StartAttacking()
    {
        IsAttacking = true;
        //_animator.SetBool(IsAttackingHash, true);
        _timer = 0;
        _isFirstAttackMade = false;
        DisableWalking();
    }

    private void StopAttacking()
    {
        IsAttacking = false;
        //_animator.SetBool(IsAttackingHash, false);
        SetSpeed(_walkSpeed);
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
        _timer = 0;
    }

    private void EnableWalking()
    {
        _navAgent.enabled = true;
        //_animator.SetBool(IsWalkingHash, true);
    }
    private void DisableWalking()
    {
        _navAgent.enabled = false;
        //_animator.SetBool(IsWalkingHash, false);
    }

    private void EnableCollider()
    {
        _collider.enabled = true;
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
    }

    private void SetSpeed(float speed)
    {
        _navAgent.speed = speed;
    }

    private float GetDistanceToTarget()
    {
        return Vector3.Distance(transform.position, _targetTransform.position);
    }

    public void SetTarget(Transform targetTransform)
    {
        _targetTransform = targetTransform;
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

    public void OnHit(Vector3 hitPoint)
    {
        throw new System.NotImplementedException();
    }
}
