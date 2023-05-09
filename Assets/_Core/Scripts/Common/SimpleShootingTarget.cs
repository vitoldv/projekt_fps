using _Core.Player;
using UnityEngine;

public class SimpleShootingTarget : MonoBehaviour, IShootingTarget
{
    public void OnHit(Vector3 hitPoint, float damage, DamageType damageType)
    {
        //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.position = hitPoint;
        //sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
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
