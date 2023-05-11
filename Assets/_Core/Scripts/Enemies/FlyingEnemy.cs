using _Core.Common;
using _Core.Player;
using _Core.Spawners;
using System.Collections;
using UnityEngine;

namespace _Core.Enemies
{
    public class FlyingEnemy : EnemyBase
    {
        private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private readonly int IsDeadHash = Animator.StringToHash("IsDead");
        private readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private readonly int WasHitHash = Animator.StringToHash("WasHit");

        [SerializeField] private float corpseLifetime = 3f;
        [SerializeField] private float movingSpeed = 5f;
        [SerializeField] private float maxDistanceFromPlayer = 10f;
        [SerializeField] private float heightAboveGround = 5f;
        [SerializeField] private float raycastDistance = 10f;
        [SerializeField] private LayerMask eyecheckRaycastLayers;
        [SerializeField] private float firstAttackDelay;
        [SerializeField] private float repeatedAttackDelay;
        [SerializeField] private float attackDamage;
        [SerializeField] private float projectileSpeed;        
        [SerializeField] private Transform projectileSpawnPoint;

        [Header("Components references")]
        [SerializeField] private Animator animator;

        public bool IsAttacking { get; private set; }

        private bool isFirstAttackMade;
        private float timer;

        public override void Init(Transform target)
        {            
            navAgent.autoTraverseOffMeshLink = false;
            navAgent.baseOffset = heightAboveGround;
            navAgent.updateRotation = false;
            navAgent.stoppingDistance = maxDistanceFromPlayer / 2;
            base.Init(target);
            EnableMoving();
            SetSpeed(movingSpeed);
        }

        private bool IsMoving => navAgent.enabled;

        private void SetSpeed(float speed)
        {
            navAgent.speed = speed;
        }

        void Update()
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
            LookOnTarget();
            if(IsMoving)
            {
                UpdateTargetPosition();
            }            
            CheckIsAttacking();
            if (IsAttacking)
            {
                HandleAttack();
            }
        }

        protected override void Die()
        {
            animator.SetBool(IsDeadHash, true);
            IsDead = true;
            StartCoroutine(C_FallOnGroud());
        }

        private void EnableMoving()
        {
            navAgent.enabled = true;
            animator.SetBool(IsMovingHash, true);
        }
        private void DisableMoving()
        {
            navAgent.enabled = false;
            animator.SetBool(IsMovingHash, false);
        }

        protected IEnumerator C_FallOnGroud()
        {
            while(navAgent.baseOffset >= -0.74)
            {
                navAgent.baseOffset -= 1.0f * Time.deltaTime;
                yield return null;
            }
        }

        public override void OnHit(Vector3 hitPoint, float damage, DamageType damageType)
        {
            animator.SetTrigger(WasHitHash);
            base.OnHit(hitPoint, damage, damageType);
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
            //if (IsTargetInSight(out var distanceToTarget) && distanceToTarget <= maxDistanceFromPlayer && !IsAttacking)
            //{
            //    StartAttacking();
            //}
            //else if (IsAttacking)
            //{
            //    StopAttacking();
            //}
            print("Check is attacking");
            if(IsTargetInSight(out var distanceToTarget))
            {
                print("In sight");
                float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetTransform.position.x, targetTransform.position.z));
                if(dist <= maxDistanceFromPlayer)
                {
                    print("distance is good " + dist);
                    if(!IsAttacking)
                    {
                        StartAttacking();
                    }
                }
            }
            else if (IsAttacking)
            {                
                StopAttacking();
            }
        }

        private bool IsTargetInSight(out float distanceToTarget)
        {
            if (Physics.Raycast(transform.position, projectileSpawnPoint.position - transform.position, out var rayInfo, raycastDistance, eyecheckRaycastLayers))
            {
                if (Utils.CheckCollision(rayInfo.collider, GameManager.GameLayers.Player))
                {
                    distanceToTarget = rayInfo.distance;
                    return true;
                }
            }
            distanceToTarget = 0;
            return false;
        }

        private void StartAttacking()
        {
            print("Start attacking");
            IsAttacking = true;
            animator.SetBool(IsAttackingHash, true);
            timer = 0;
            isFirstAttackMade = false;
            DisableMoving();
        }

        private void StopAttacking()
        {
            print("Stop attacking");
            IsAttacking = false;
            animator.SetBool(IsAttackingHash, false);
            EnableMoving();
        }

        private void Attack()
        {
            if (targetTransform.TryGetComponent(out PlayerController player))
            {
                var projectile = EnemySpawner.SpawnInsectProjectile(projectileSpawnPoint.position);
                projectile.Init(targetTransform.position - transform.position, projectileSpeed, attackDamage);
            }
            timer = 0;
        }

        private void OnGUI()
        {
            if (IsDead) return;
            //GUI.Label(new Rect(50, 50, 400, 400), info);
        }
    }
}