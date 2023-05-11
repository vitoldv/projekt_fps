using System;
using UnityEngine;
using UnityEngine.AI;
using _Core.Player;
using _Core.Spawners;

namespace _Core.Enemies
{

    [RequireComponent(typeof(CapsuleCollider), typeof(NavMeshAgent))]
    public abstract class EnemyBase : MonoBehaviour, IShootingTarget, IPoolableObject
    {
        public event Action<EnemyBase> Defeated;

        [SerializeField] protected float rotationSpeed;
        [SerializeField] private float initialHp;
        protected Collider collider;
        protected NavMeshAgent navAgent;
        protected Transform targetTransform;

        public float HP { get; protected set; }
        public bool IsDead { get; protected set; }

        private void Awake()
        {
            collider = GetComponent<CapsuleCollider>();
            navAgent = GetComponent<NavMeshAgent>();
        }

        public virtual void Init(Transform target)
        {
            this.targetTransform = target;
            HP = initialHp;
        }

        protected void EnableCollider()
        {
            collider.enabled = true;
        }

        protected void DisableCollider()
        {
            collider.enabled = false;
        }

        protected float GetDistanceToTarget()
        {
            return Vector3.Distance(transform.position, targetTransform.position);
        }

        public void ReceiveDamage(float damageAmount)
        {
            HP -= damageAmount;
            if (HP <= 0)
            {
                Die();
                Defeated?.Invoke(this);
            }
        }

        protected void LookOnTarget()
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        protected void UpdateTargetPosition()
        {
            navAgent.SetDestination(targetTransform.position);
        }

        protected abstract void Die();

        public virtual void OnHit(Vector3 hitPoint, float damage, DamageType damageType)
        {
            ReceiveDamage(damage);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            IsDead = false;
            targetTransform = null;
            gameObject.SetActive(false);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}