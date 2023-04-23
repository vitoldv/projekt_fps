using UnityEngine;
using Utils = _Core.Utils;

public class ProjectileBase : MonoBehaviour
{
    public LayerMask onDestroyLayers;
    private Vector3 direction;
    private float speed;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
    // Start is called before the first frame update
    public void Init(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("enter");
        if (Utils.CheckCollision(other, onDestroyLayers))
        {
            print("yes");
            Destroy(gameObject);
        }
    }
}
