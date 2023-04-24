using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PoolBall : MonoBehaviour
{
    [SerializeField] PoolTable.Team team;
    [SerializeField] bool isWhiteBall = false;
    [SerializeField] bool isEightBall = false;

    Rigidbody rb;
    Vector3 startPosition;

    public bool IsRolling => rb.velocity.magnitude >= 0.01f;
    public bool IsWhiteBall => isWhiteBall;
    public bool IsEightBall => isEightBall;
    public PoolTable.Team Team => team;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        transform.rotation = Random.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Shoot(Vector3 direction, Vector3 position)
    {
        rb.AddForceAtPosition(direction, position, ForceMode.Impulse);
    }
}