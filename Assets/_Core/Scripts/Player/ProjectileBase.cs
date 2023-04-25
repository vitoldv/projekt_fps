using _Core;
using Assets._Core.Scripts.Player;
using UnityEngine;
using Utils = _Core.Utils;

public class ProjectileBase : MonoBehaviour, IPoolableObject
{
    public LayerMask onDestroyLayers;
    private Vector3 direction;
    private float speed;
    private float damage;
    private float explosionRadius;

    // Start is called before the first frame update
    public void Init(Vector3 direction, float speed, float damage, float explosionRadius)
    {
        this.direction = direction;
        this.speed = speed;
        this.explosionRadius = explosionRadius;
        this.damage = damage;         
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckCollision(other, onDestroyLayers))
        {
            Explode();            
        }
    }

    private void Explode()
    {
        // play some effect
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius, onDestroyLayers);
        print($"Colliders num: {colliders.Length}");
        foreach (var collider in colliders)
        {
            print($"Collider: {collider.gameObject.name}");
            if (collider.gameObject.TryGetComponent<IShootingTarget>(out var shootingTarget))
            {
                Vector3 hitPoint = collider.ClosestPoint(transform.position);
                float distanceToObject = Vector3.Distance(hitPoint, transform.position);
                // the damage is less the further the object from epicenter
                float actualDamage = damage * (1 - distanceToObject / explosionRadius);
                print($"shooting target: {actualDamage}, {distanceToObject}");
                shootingTarget.OnHit(hitPoint, actualDamage, DamageType.Explosion);
            }
        }
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
