using UnityEngine;

public class PoolTable : MonoBehaviour
{
    [SerializeField] PoolBall[] balls;

    public enum Team
    {
        None,
        Stripes,
        Solids
    }

    Team currentShooter = Team.None;

    int stripesDown = 0;
    int solidsDown = 0;

    const int BallsBeforeBlackBall = 7;

    public void ResetGame()
    {
        currentShooter = Team.None;
        stripesDown = 0;
        solidsDown = 0;

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].Restart();
        }
    }

    public bool IsReadyToShoot()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].IsRolling)
            {
                return false;
            }
        }
        return true;
    }

    public void BallDown(PoolBall ball)
    {
        switch (ball.Team)
        {
            case Team.Stripes:
                break;
            case Team.Solids:
                break;
            default:
                break;
        }
    }
}