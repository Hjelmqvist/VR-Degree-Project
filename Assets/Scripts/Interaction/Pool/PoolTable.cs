using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PoolTable : MonoBehaviour
{
    [SerializeField] PoolBall[] balls;
    [SerializeField] PoolCue[] cues;
    [SerializeField] float whiteBallRespawnDelay = 1f;
    [SerializeField] TextMeshPro winnerText;
    [SerializeField] TextMeshPro stripesText;
    [SerializeField] TextMeshPro solidsText;

    public enum Team
    {
        None,
        Stripes,
        Solids,
        EightBall
    }

    Dictionary<Team, int> teamScores = new Dictionary<Team, int>()
    {
        {Team.Stripes, 0},
        {Team.Solids, 0}
    };

    Dictionary<Team, int> turnScores = new Dictionary<Team, int>()
    {
        {Team.Stripes, 0},
        {Team.Solids, 0}
    };

    Team currentShooter = Team.None;

    bool shotWhiteBall = false;
    bool gameIsOver = false;

    const int BallsBeforeBlackBall = 7;
    const int WhiteBallIndex = 0;

    private void Update()
    {
        if (shotWhiteBall && IsReadyToShoot())
        {
            NextTurn();
        }
    }

    public void ResetGame()
    {
        currentShooter = Team.None;
        gameIsOver = true;
        teamScores[Team.Stripes] = 0;
        teamScores[Team.Solids] = 0;
        turnScores[Team.Stripes] = 0;
        turnScores[Team.Solids] = 0;
        UpdateScoreText();
        winnerText.gameObject.SetActive(false);

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

    public void WhiteBallShot()
    {
        shotWhiteBall = true;
    }

    private void NextTurn()
    {
        shotWhiteBall = false;

        if (currentShooter != Team.None && turnScores[currentShooter] > 0)
        {

        }

        // Change to the other team
        if (currentShooter != Team.None)
        {
            currentShooter = currentShooter == Team.Stripes ? Team.Solids : Team.Stripes;
        }

        if (!balls[WhiteBallIndex].gameObject.activeInHierarchy)
        {
            balls[WhiteBallIndex].ResetPosition();
        }

        turnScores[Team.Stripes] = 0;
        turnScores[Team.Solids] = 0;
    }

    public void BallDown(PoolBall ball)
    {
        switch (ball.Team)
        {
            case Team.Stripes:
            case Team.Solids:
                if (currentShooter == Team.None)
                {
                    currentShooter = ball.Team;
                }
                teamScores[ball.Team]++;
                turnScores[ball.Team]++;
                UpdateScoreText();
                break;

            case Team.EightBall:
                EightBallDown(ball);
                break;
        }
    }

    private void UpdateScoreText()
    {
        stripesText.text = teamScores[Team.Stripes].ToString();
        solidsText.text = teamScores[Team.Solids].ToString();
    }

    private void EightBallDown(PoolBall ball)
    {
        Team winningTeam = Team.None;

        if (currentShooter == Team.None || teamScores[currentShooter] == BallsBeforeBlackBall)
        {
            EndGame(currentShooter);
        }
        else
        {
            winningTeam = currentShooter == Team.Stripes ? Team.Solids : Team.Stripes;
            EndGame(winningTeam);
        }
    }

    private void EndGame(Team winningTeam)
    {
        if (winningTeam == Team.None)
        {
            ResetGame();
        }
        else
        {
            gameIsOver = true;
            winnerText.text = $"{winningTeam} won!";
            winnerText.gameObject.SetActive(true);
        }
    }
}