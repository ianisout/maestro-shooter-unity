using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    EnemySpawner enemySpawner;
    WaveConfigSO waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;
    float moveSpeed; 
    int waveCount;
    int hasWaveIncrease = 1;

    void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    void Start()
    {
        waveConfig = enemySpawner.GetCurrentWave();
        moveSpeed = waveConfig.GetMoveSpeed();
        waveCount = enemySpawner.GetWaveCount();
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointIndex].position;
    }

    void Update()
    {
        FollowPath();
    }

    void FollowPath()
    {
        float delta = IncreaseDifficulty() * Time.deltaTime;

        if(waypointIndex < waypoints.Count)
        {
            Vector3 targetPosition = waypoints[waypointIndex].position;

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, delta);

            if(transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float IncreaseDifficulty()
    {
        if (waveCount != hasWaveIncrease)
        {
            moveSpeed += 0.2f;
            hasWaveIncrease += 1;
        } 

        return moveSpeed;
    }
}
