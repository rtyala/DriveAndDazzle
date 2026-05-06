using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 5;
    public float spawnInterval = 2f;

    public Transform leftLane;
    public Transform rightLane;

    private List<GameObject> enemyPool;
    private float timer;

    void Start()
    {
        enemyPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Transform selectedLane = Random.Range(0, 2) == 0 ? leftLane : rightLane;
                Vector3 spawnPosition = new Vector3(selectedLane.position.x, transform.position.y, transform.position.z);

                enemy.transform.position = spawnPosition;
                enemy.SetActive(true);
                return;
            }
        }
    }
}