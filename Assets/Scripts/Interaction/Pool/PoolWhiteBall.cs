using UnityEngine;

public class PoolWhiteBall : PoolBall
{
    [SerializeField] PoolTable table;

    public void Shoot(Vector3 direction, Vector3 position)
    {
        if (table.IsReadyToShoot())
        {
            rb.AddForceAtPosition(direction, position, ForceMode.Impulse);
            table.WhiteBallShot();
        }
    }
}