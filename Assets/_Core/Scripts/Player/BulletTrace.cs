using _Core;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrace : MonoBehaviour, IPoolableObject
{
    public float maxShootingDistance;
    public float timeAlive;

    private LineRenderer bulletTraceLineRenderer;
    private Material material;

    private bool isInitialized;
    private float timePassed = 0f;

    private void Awake()
    {
        bulletTraceLineRenderer = GetComponent<LineRenderer>();
        material = GetComponent<Renderer>().material;
    }
    
    public void Init(Vector3 shootingPoint, Vector3 shootingDir, Vector3 hitPoint = default)
    {
        bulletTraceLineRenderer.SetPosition(0, shootingPoint);
        if(hitPoint != default)
        {
            bulletTraceLineRenderer.SetPosition(1, hitPoint);
        }
        else
        {
            bulletTraceLineRenderer.SetPosition(1, shootingPoint + shootingDir * maxShootingDistance);
        }
        isInitialized = true;
    }


    private void Update()
    {
        if (!isInitialized) return;

        timePassed += Time.deltaTime;
        if (timePassed >= timeAlive)
        {
            Disable();
        }

        var color = material.color;
        color.a = color.a - (100 / timeAlive * Time.deltaTime);
        material.color = color;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        isInitialized = false;
        timePassed = 0;
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
