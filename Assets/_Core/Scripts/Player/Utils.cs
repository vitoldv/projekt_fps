using UnityEngine;

namespace _Core
{
    public class Utils
    {
        public static bool CheckCollision(Collider collider, LayerMask collisionMask)
        {
            int mask = 1 << collider.gameObject.layer;
            return (collisionMask.value & mask) != 0;
        }
        
        public static bool GetRandomBool()
        {
            return !(Random.Range(-2F, 1F) < 0);
        }
        
        public static Vector3 GetDirectionOnPoint(Vector3 fromPosition, Vector3 toPosition)
        {
            return (toPosition - fromPosition).normalized;
        }
    }
}