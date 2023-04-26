using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : EnemyBase
{
    public float speed = 5f;
    public float distanceFromPlayer = 10f;
    public float heightAboveGround = 5f;
    public float raycastDistance = 10f;

    public override void Init(Transform target)
    {
        navAgent.autoTraverseOffMeshLink = false;
        navAgent.baseOffset = heightAboveGround;
        navAgent.updateRotation = false;
        navAgent.stoppingDistance = distanceFromPlayer / 2;
        base.Init(target);
    }

    void Update()
    {
        // Calculate the target position to follow the player while maintaining a certain distance
        Vector3 playerPosition = targetTransform.position;
        Vector3 enemyPosition = transform.position;
        Vector3 directionToPlayer = playerPosition - enemyPosition;
        Vector3 targetPosition = playerPosition - directionToPlayer.normalized * distanceFromPlayer;

        // Cast a ray from the target position to the enemy position to check for walls
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, enemyPosition - targetPosition, out hit, raycastDistance))
        {
            // If a wall is hit, set the target position to the point where the ray hit the wall
            targetPosition = hit.point;
        }

        // Move towards the target position while maintaining a certain height above the ground
        Vector3 targetDirection = (targetPosition - enemyPosition).normalized;
        Vector3 targetHeight = Vector3.up * heightAboveGround;
        var destination = targetPosition - targetDirection * 2.0f + targetHeight;
        NavMesh.SamplePosition(destination, out NavMeshHit hitNavMesh, 10.0f, NavMesh.AllAreas);
        destination = hitNavMesh.position;

        // Check if there is an off-mesh link (such as a jump) in the way
        if (navAgent.isOnOffMeshLink)
        {
            // If so, move up or down to reach the next part of the link
            if (transform.position.y > navAgent.currentOffMeshLinkData.endPos.y)
            {
                destination = new Vector3(destination.x, transform.position.y - heightAboveGround, destination.z);
            }
            else
            {
                destination = new Vector3(destination.x, navAgent.currentOffMeshLinkData.endPos.y + heightAboveGround, destination.z);
            }
        }
        else
        {
            // Otherwise, move along the navmesh normally
            navAgent.SetDestination(destination);
        }

        // Rotate towards the player
        transform.LookAt(playerPosition);
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
