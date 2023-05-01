using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PoolTable : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    [SerializeField] PoolBall[] balls;
    [SerializeField] PoolCue[] cues;
    [SerializeField] float whiteBallRespawnDelay = 1f;
    

    public enum Team
    {
        None,
        Stripes,
        Solids
    }

    Dictionary<Team, int> teamScores = new Dictionary<Team, int>()
    {
        {Team.Stripes, 0},
        {Team.Solids, 0}
    };

    Team currentShooter = Team.None;

    bool shotWhiteBall = false;
    bool gameIsOver = false;

    const int BallsBeforeBlackBall = 7;

    private void Update()
    {
        if (shotWhiteBall && IsReadyToShoot())
        {
            
        }
    }

    public void ResetGame()
    {
        currentShooter = Team.None;
        gameIsOver = true;
        teamScores[Team.Stripes] = 0;
        teamScores[Team.Solids] = 0;

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].ResetPosition();
        }
    }

    public void ResetCues()
    {
        for (int i = 0; i < cues.Length; i++)
        {
            cues[i].ResetPosition();
        }
    }

    public bool IsReadyToShoot()
    {
        if (gameIsOver)
        {
            return false;
        }

        for (int i = 0; i < balls.Length; i++)
        {
            if (balls[i].IsRolling)
            {
                return false;
            }
        }
        return true;
    }

    private void NextTurn()
    {
        // Change to the other team
        if (currentShooter != Team.None)
        {
            currentShooter = currentShooter == Team.Stripes ? Team.Solids : Team.Stripes;
        }

        // Reset white ball
        if (!balls[0].gameObject.activeInHierarchy)
        {
            balls[0].ResetPosition();
        }
    }

    public void BallDown(PoolBall ball)
    {
        switch (ball.Team)
        {
            case Team.None:
                if (ball.IsWhiteBall)
                {
                    WhiteBallDown(ball);
                }
                else if (ball.IsEightBall)
                {
                    EightBallDown(ball);
                }
                break;

            case Team.Stripes:
            case Team.Solids:
                teamScores[ball.Team]++;
                break;
        }
    }

    private void WhiteBallDown(PoolBall ball)
    {
        StartCoroutine(WhiteBallRespawn());

        IEnumerator WhiteBallRespawn()
        {
            yield return new WaitUntil(() => IsReadyToShoot());
            yield return new WaitForSeconds(whiteBallRespawnDelay);
            ball.ResetPosition();
        }
    }

    private void EightBallDown(PoolBall ball)
    {
        Team winningTeam = Team.None;

        // Win if last ball
        if (currentShooter != Team.None && teamScores[currentShooter] == BallsBeforeBlackBall)
        {
            Win(currentShooter);
        }
        else // Other team wins if eight ball went down before all team balls
        {
            if (currentShooter != Team.None)
            {
                winningTeam = currentShooter == Team.Stripes ? Team.Solids : Team.Stripes;
            }
            Win(winningTeam);
        }
    }

    private void Win(Team winningTeam)
    {
        gameIsOver = true;
        //text.text = $"{winningTeam} won!";
    }
}