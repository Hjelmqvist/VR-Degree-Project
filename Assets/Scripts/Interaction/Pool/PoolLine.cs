using UnityEngine;

public class PoolLine : MonoBehaviour
{
    [SerializeField] Transform cueTip;
    [SerializeField] LineRenderer lineToBall;
    [SerializeField] LineRenderer lineBehindBall;
    [SerializeField] float distance = 0.2f;
    [SerializeField] float lineToBallDistance = 0.05f;
    [SerializeField] float lineBehindBallDistance = 0.1f;

    private void Update()
    {
        if (Physics.Raycast(cueTip.position, cueTip.forward, out RaycastHit hit, distance) &&
            hit.transform.TryGetComponent(out PoolWhiteBall ball))
        {
            lineToBall.SetPosition(0, hit.point);
            lineToBall.SetPosition(1, transform.position);

            Vector3 direction = ball.transform.position - hit.point;
            float length = direction.magnitude;
            direction.y = 0;
            direction.Normalize();
            lineBehindBall.SetPosition(0, ball.transform.position + direction * length);
            lineBehindBall.SetPosition(1, ball.transform.position + direction * lineBehindBallDistance);
        }
        else
        {
            lineToBall.SetPosition(0, Vector3.zero);
            lineToBall.SetPosition(1, Vector3.zero);
            lineBehindBall.SetPosition(0, Vector3.zero);
            lineBehindBall.SetPosition(1, Vector3.zero);
        }
    }

    private void OnDisable()
    {
        lineToBall.SetPosition(0, Vector3.zero);
        lineToBall.SetPosition(1, Vector3.zero);
        lineBehindBall.SetPosition(0, Vector3.zero);
        lineBehindBall.SetPosition(1, Vector3.zero);
    }
}