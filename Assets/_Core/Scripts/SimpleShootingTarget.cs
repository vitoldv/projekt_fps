using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShootingTarget : MonoBehaviour, IShootingTarget
{
    public void OnHit(Vector3 hitPoint)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = hitPoint;
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}