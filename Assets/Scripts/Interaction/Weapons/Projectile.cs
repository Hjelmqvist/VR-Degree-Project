using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float distanceLimit = 100;
    [SerializeField] TrailRenderer trail;

    Rigidbody rb;
    Vector3 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > distanceLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        trail.transform.SetParent(null);
    }
}