using UnityEngine;

public class PoolCueTip : MonoBehaviour
{
    [SerializeField] Rigidbody poolCue;
    [SerializeField] float shootMultiplier = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoolWhiteBall ball))
        {
            ball.Shoot(poolCue.velocity * shootMultiplier, transform.position);
        }
    }
}