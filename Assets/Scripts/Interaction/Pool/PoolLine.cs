using UnityEngine;

public class PoolLine : MonoBehaviour
{
    [SerializeField] Transform cueTip;
    [SerializeField] LineRenderer lineToBall;
    [SerializeField] LineRenderer lineBehindBall;
    [SerializeField] float lineDistance = 0.1f;

    private void Update()
    {
        if (Physics.Raycast(cueTip.position, cueTip.forward, out RaycastHit hit, lineDistance) &&
            hit.transform.TryGetComponent(out PoolBall ball) && ball.IsWhiteBall)
        {
            lineToBall.SetPosition(0, cueTip.position);
            lineToBall.SetPosition(1, hit.point);

            Vector3 direction = ball.transform.position - hit.point;
            float length = direction.magnitude;
            direction.Normalize();
            lineBehindBall.SetPosition(0, ball.transform.position + direction * length);
            lineBehindBall.SetPosition(1, ball.transform.position + direction * lineDistance);
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