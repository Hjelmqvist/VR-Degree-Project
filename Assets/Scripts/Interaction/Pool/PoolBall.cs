using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PoolBall : MonoBehaviour
{
    [SerializeField] PoolTable.Team team;
    [SerializeField] bool isWhiteBall = false;
    [SerializeField] bool isEightBall = false;

    Rigidbody rb;
    bool isRolling = false;
    Vector3 startPosition;

    public bool IsRolling => isRolling;
    public bool IsWhiteBall => isWhiteBall;
    public bool IsEightBall => isEightBall;
    public PoolTable.Team Team => team;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ResetPosition();
    }

    private void Update()
    {
        isRolling = rb.velocity.magnitude <= 0.01f;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Random.rotation;
        gameObject.SetActive(true);
        rb.velocity = Vector3.zero;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        rb.velocity = Vector3.zero;
    }

    public void Shoot(Vector3 direction, Vector3 position)
    {
        rb.AddForceAtPosition(direction, position, ForceMode.Impulse);
    }
}