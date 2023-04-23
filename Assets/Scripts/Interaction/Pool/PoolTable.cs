using UnityEngine;
using TMPro;
using System.Collections;

public class PoolTable : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] PoolBall[] balls;
    [SerializeField] float whiteBallRespawnDelay = 1f;

    public enum Team
    {
        None,
        Stripes,
        Solids
    }

    Team currentShooter = Team.None;
    int stripesDown = 0;
    int solidsDown = 0;
    bool gameIsOver = false;

    const int BallsBeforeBlackBall = 7;

    public void ResetGame()
    {
        currentShooter = Team.None;
        stripesDown = 0;
        solidsDown = 0;
        gameIsOver = true;

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].ResetPosition();
        }
    }

    public bool IsReadyToShoot()
    {
        if (gameIsOver)
        {
            Debug.Log("Game over");
            return false;
        }

        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].IsRolling)
            {
                Debug.Log(balls[i] + " is rolling");
                return false;
            }
        }
        return true;
    }

    public void BallDown(PoolBall ball)
    {
        switch (ball.Team)
        {
            case Team.None:
                if (ball.IsWhiteBall)
                {
                    WantAndRespawnWhiteBall(ball);
                }
                else if (ball.IsEightBall)
                {
                    if ((currentShooter == Team.Stripes && stripesDown == BallsBeforeBlackBall) || 
                        (currentShooter == Team.Solids && solidsDown == BallsBeforeBlackBall))
                    {
                        Win(currentShooter);
                    }
                    else
                    {
                        Win(currentShooter == Team.Stripes ? Team.Solids : Team.Stripes);
                    }    
                }
                break;

            case Team.Stripes:
                stripesDown++;
                break;

            case Team.Solids:
                solidsDown++;
                break;
        }
    }

    private void WantAndRespawnWhiteBall(PoolBall ball)
    {
        StartCoroutine(WhiteBallRespawn());

        IEnumerator WhiteBallRespawn()
        {
            yield return new WaitUntil(() => IsReadyToShoot());
            yield return new WaitForSeconds(whiteBallRespawnDelay);
            ball.ResetPosition();
        }
    }

    private void Win(Team winningTeam)
    {
        gameIsOver = true;
        text.text = $"{winningTeam} won!";
    }
}