using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTable : MonoBehaviour
{
    [SerializeField] Transform[] balls;
    Vector3[] ballStartPositions;

    private void Awake()
    {
        ballStartPositions = new Vector3[balls.Length];
        for (int i = 0; i < balls.Length; i++)
        {
            ballStartPositions[i] = balls[i].position;
        }
        ResetGame();
    }

    public void ResetGame()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].position = ballStartPositions[i];
            balls[i].gameObject.SetActive(true);
        }
    }
}
