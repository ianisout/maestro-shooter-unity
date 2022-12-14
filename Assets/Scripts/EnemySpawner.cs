using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfigSO> waveConfigs;
    [SerializeField] float timebetweenWaves = 0f;
    [SerializeField] bool isLooping = false;
    WaveConfigSO currentWave;
    int waveCount = 1;

    void Start()
    {
        StartCoroutine(SpawnEnemyWaves());
    }

    public WaveConfigSO GetCurrentWave()
    {
        return currentWave;
    }

    IEnumerator SpawnEnemyWaves()
    {
        do {
            foreach (WaveConfigSO wave in waveConfigs)
            {
                var random = new System.Random();
                currentWave = wave;

                for (int i = 0; i < (currentWave.GetEnemyCount() - random.Next(9, 11)); i++)
                {
                    int randomEnemyIndex = random.Next(currentWave.GetEnemyCount());

                    Instantiate(currentWave.GetEnemyPrefab(randomEnemyIndex),
                                currentWave.GetStartingWaypoint().position,
                                Quaternion.identity,
                                transform);

                    yield return new WaitForSeconds(currentWave.GetRandomSpawnTime());
                }

                waveCount++;
            }

            yield return new WaitForSeconds(timebetweenWaves);

        }
        while (isLooping);
    }

    public int GetWaveCount()
    {
        return waveCount;
    }
}
