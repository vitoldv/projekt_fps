using UnityEngine;

namespace Assets._Core.Scripts.Enemies
{
    public class WalkingEnemy : EnemyBase
    {
        // Animator properties hashes
        private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

        [Header("General")]
        [SerializeField] private float initialHp;
        [SerializeField] private float corpseLifetime;
        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        [Header("Attack")]
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackDistance;
        [SerializeField] private float firstAttackDelay;
        [SerializeField] private float repeatedAttackDelay;

        public bool IsAttacking { get; private set; }
        public bool IsWalking => navAgent.enabled;

        private bool isFirstAttackMade;
        private float timer;

        public override void Init(Transform target)
        {            
            SetSpeed(walkSpeed);
            EnableWalking();
            EnableCollider();
            HP = initialHp;
            timer = 0;
            isFirstAttackMade = false;
            IsDead = false;

            base.Init(target);
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

        protected override void Die()
        {
            IsDead = true;
            //_animator.SetBool(IsDeadHash, true);
            DisableCollider();
            DisableWalking();
            StopAttacking();
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

        private void SetSpeed(float speed)
        {
            navAgent.speed = speed;
        }
    }
}
