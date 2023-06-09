﻿using _Core.Player;
using UnityEngine;

namespace _Core.Enemies
{
    public class WalkingEnemy : EnemyBase
    {
        // Animator properties hashes
        private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private readonly int WasHitHash = Animator.StringToHash("WasHit");

        [Header("General")]

        [SerializeField] private float corpseLifetime;
        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        [Header("Attack")]
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackDistance;
        [SerializeField] private float firstAttackDelay;
        [SerializeField] private float repeatedAttackDelay;

        [SerializeField] private Animator animator;
        // DEBUG
        [SerializeField] private EnemyHealth healthBar;

        public bool IsAttacking { get; private set; }
        public bool IsWalking => navAgent.enabled;

        private bool isFirstAttackMade;
        private float timer;

        public override void Init(Transform target)
        {            
            SetSpeed(walkSpeed);
            EnableWalking();
            EnableCollider();

            timer = 0;
            isFirstAttackMade = false;
            IsDead = false;

            base.Init(target);
        }

        private void Update()
        {
            healthBar.SetHealth(this.HP);
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
            animator.SetBool(IsAttackingHash, true);
            timer = 0;
            isFirstAttackMade = false;
            DisableWalking();
        }

        private void StopAttacking()
        {
            IsAttacking = false;
            animator.SetBool(IsAttackingHash, false);
            SetSpeed(walkSpeed);
            EnableWalking();
        }

        protected override void Die()
        {
            IsDead = true;
            DisableWalking();
            StopAttacking();
            animator.SetBool(IsDeadHash, true);
            DisableCollider();
            timer = 0;
        }

        private void EnableWalking()
        {
            navAgent.enabled = true;
            animator.SetBool(IsWalkingHash, true);
        }
        private void DisableWalking()
        {
            navAgent.enabled = false;
            animator.SetBool(IsWalkingHash, false);
        }

        private void SetSpeed(float speed)
        {
            navAgent.speed = speed;
        }

        public override void OnHit(Vector3 hitPoint, float damage, DamageType damageType)
        {
            animator.SetTrigger(WasHitHash);
            base.OnHit(hitPoint, damage, damageType);
        }
    }
}
