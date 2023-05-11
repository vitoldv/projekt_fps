using _Core.Spawners;
using UnityEngine;

namespace _Core.Common
{
    public abstract class ProjectileBase : MonoBehaviour, IPoolableObject
    {
        [SerializeField] protected LayerMask onDestroyLayers;
        protected Vector3 direction;
        protected float speed;
        protected float damage;

        public void Init(Vector3 direction, float speed, float damage)
        {
            this.direction = direction;
            this.speed = speed;
            this.damage = damage;
        }

        protected void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (Utils.CheckCollision(other, onDestroyLayers))
            {
                OnHit(other);
            }
        }

        protected virtual void OnHit(Collider collider)
        {
            Disable();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            direction = Vector3.zero;
            speed = 0;
            gameObject.SetActive(false);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}


