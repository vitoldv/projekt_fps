using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrace : MonoBehaviour
{
    public float maxShootingDistance;
    public float timeAlive;
    private LineRenderer bulletTraceLineRenderer;
    private Material material;
    private bool isInitialized;
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
        //StartCoroutine(C_DestroyInSeconds(timeAlive));
        isInitialized = true;
    }

    private float timePassed = 0f;
    private void Update()
    {
        if (!isInitialized) return;

        timePassed += Time.deltaTime;
        if (timePassed >= timeAlive)
        {
            Destroy(gameObject);

        }

        var color = material.color;
        color.a = color.a - (100 / timeAlive * Time.deltaTime);
        material.color = color;
    }
}
