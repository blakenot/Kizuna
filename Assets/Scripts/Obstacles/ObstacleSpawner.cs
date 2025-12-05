using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform[] laneSpawnPoints;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float minInterval = 0.6f;
    [SerializeField] private float maxInterval = 1.6f;
    [SerializeField] private float spawnHeight = 10f;

    void Start()
    {
        if (laneSpawnPoints == null || laneSpawnPoints.Length == 0)
        {
            Debug.LogError("ObstacleSpawner: No lane spawn points assigned!");
            enabled = false;
            return;
        }

        if (obstaclePrefab == null)
        {
            Debug.LogError("ObstacleSpawner: No obstaclePrefab assigned!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (!GameManager.Instance.IsGameOver)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        int lane = Random.Range(0, laneSpawnPoints.Length);
        Transform spawnPoint = laneSpawnPoints[lane];

        Vector3 spawnPos = spawnPoint.position + Vector3.up * spawnHeight;

        GameObject obj = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        Obstacle ob = obj.GetComponent<Obstacle>();
        if (ob != null)
        {
            ob.Initialize(GetRandomColor());
        }
        else
        {
            Debug.LogError("Obstacle prefab missing Obstacle component!");
        }
    }

    TrackSegment.ColorState GetRandomColor()
    {
        float r = Random.value;
        if (r < 0.4f) return TrackSegment.ColorState.White;
        else if (r < 0.55f) return TrackSegment.ColorState.Red;
        else if (r < 0.70f) return TrackSegment.ColorState.Green;
        else if (r < 0.85f) return TrackSegment.ColorState.Blue;
        else return TrackSegment.ColorState.Black;
    }
}