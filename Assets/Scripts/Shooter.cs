using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    [Header("General")]
    [SerializeField] List<GameObject> projectilePrefabs;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float baseFiringRate = 0.2f;

    [Header("AI")]
    [SerializeField] bool useAI;
    [SerializeField] float firingRateVariance = 0f;
    [SerializeField] float minimumFiringRate = 0.1f;

    [HideInInspector] public bool isFiring;
    [HideInInspector] public bool isReflecting;

    PlayerSpecial playerSpecial;
    GameObject[] enemyNotes;
    Coroutine firingCoroutine;
    AudioPlayer audioPlayer;
    UIDisplay uIDisplay;
    PathFinder pathFinder;
    EnemySpawner enemySpawner;

    int waveCount;
    float timeToNextProjectile;

    void Awake()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        playerSpecial = FindObjectOfType<PlayerSpecial>();
        uIDisplay = FindObjectOfType<UIDisplay>();
        pathFinder = FindObjectOfType<PathFinder>();
        enemySpawner = FindObjectOfType<EnemySpawner>();        
    }

    void Start()
    {
        waveCount = enemySpawner.GetWaveCount();

        if (useAI)
        {
            isFiring = true;
        }
    }

    void Update()
    {
        Fire();
        ReflectNotes();
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null) {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    void ReflectNotes()
    {
        enemyNotes = GameObject.FindGameObjectsWithTag("Projectile");

        for (int i = 0; i < enemyNotes.Length; i++)
        {
            if (enemyNotes[i].layer == 7 && enemyNotes[i].gameObject.tag == "Projectile" && isReflecting)
            {
                Rigidbody2D rb = enemyNotes[i].GetComponent<Rigidbody2D>();
                
                enemyNotes[i].layer = 8;

                // Destroying enemy projectile when they go past the bottom UI
                if (enemyNotes[i].transform.position.y < -4.5)
                {
                    Destroy(enemyNotes[i]);

                } else if (rb)
                {
                    rb.velocity = transform.up * projectileSpeed;
                    uIDisplay.ResetSliderColor();
                }
            }
        }
    }

    public int RandomProjectilePrefab()
    {
        int limitNumber = projectilePrefabs.Count;
        int randomIndex = Random.Range(0, limitNumber);

        return randomIndex;
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            // Setting up the notes leaving the maestro's baton
            Vector3 playerLocation = transform.position;

            if (!useAI) {
                playerLocation.x -= 0.6f;
                playerLocation.y -= 1.4f;
            }

            GameObject instance = Instantiate(projectilePrefabs[RandomProjectilePrefab()], playerLocation, Quaternion.identity);
            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

            // MAESTRO'S SHOOTING
            if (rb != null && !useAI)
            {
                rb.velocity = transform.up * projectileSpeed;
                audioPlayer.PlayShootingClip();
            }
            // ENEMYS' SHOOTING
            else if (rb != null && useAI)
            {
                if (rb.gameObject.layer == 7)
                {
                    rb.velocity = transform.up * -1 * pathFinder.IncreaseDifficulty();
                }
                else if (rb.gameObject.layer == 8)
                {
                    rb.velocity = transform.up * projectileSpeed;
                }
            }

            if (waveCount > 10)
            {
                timeToNextProjectile = Random.Range(baseFiringRate - firingRateVariance,
                                                        baseFiringRate + firingRateVariance);
        
                timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);
            }
            else
            {
                // remedying the enemy shooting not to start to fast in first waves
                var random = new System.Random();
                timeToNextProjectile = Mathf.Clamp(2, 4, float.MaxValue);
            }

            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
