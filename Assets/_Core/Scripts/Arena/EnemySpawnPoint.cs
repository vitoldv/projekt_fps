using _Core.Enemies;
using UnityEngine;

namespace _Core.Arena
{
    [RequireComponent(typeof(SphereCollider))]
    public class EnemySpawnPoint : MonoBehaviour
    {
        private EnemyType EnemyType;

        private SphereCollider sphereCollider;

        private void Awake()
        {
            sphereCollider = GetComponent<SphereCollider>();
        }

        public void PlaceEnemy(EnemyBase enemy)
        {
            enemy.transform.position = GetRandomPointOnHorizontalCircle(sphereCollider);
        }

        private Vector3 GetRandomPointOnHorizontalCircle(SphereCollider sphereCollider)
        {
            // Get the center of the sphere collider
            Vector3 center = sphereCollider.transform.position + sphereCollider.center;

            // Get the radius of the sphere collider
            float radius = sphereCollider.radius;

            // Generate a random angle in radians
            float angle = Random.Range(0f, Mathf.PI * 2);

            // Calculate the x and z coordinates of the point on the circle
            float x = center.x + radius * Mathf.Cos(angle);
            float z = center.z + radius * Mathf.Sin(angle);

            // Return the point as a Vector3
            return new Vector3(x, center.y, z);
        }
    }
}
