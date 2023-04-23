using UnityEngine;

public class PoolHole : MonoBehaviour
{
    [SerializeField] PoolTable table;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoolBall ball))
        {
            ball.Deactivate();
            table.BallDown(ball);
        }
    }
}