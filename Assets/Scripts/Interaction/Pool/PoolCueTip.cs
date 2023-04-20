using UnityEngine;

public class PoolCueTip : MonoBehaviour
{
    [SerializeField] Rigidbody poolCue;
    [SerializeField] PoolTable poolTable;
    [SerializeField] float shootMultiplier = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoolBall ball) && ball.IsWhiteBall && poolTable.IsReadyToShoot())
        {
            ball.Shoot(poolCue.velocity * shootMultiplier, transform.position);
        }
    }
}