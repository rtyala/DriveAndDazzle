using UnityEngine;
using System.Collections.Generic;

public class EndlessRoadGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject roadSegmentPrefab;   // Твой готовый префаб RoadSegment

    [Header("Настройки")]
    public int poolSize = 4;               // Количество сегментов в пуле
    public float segmentLength = 20f;      // Длина твоего сегмента
    public float deleteDistance = -40f;    // На каком Z удалять

    private List<GameObject> segments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateSegment(nextSpawnZ);
            nextSpawnZ += segmentLength;
        }
    }

    void Update()
    {
        // Удаляем старые сегменты
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (segments[i].transform.position.z < deleteDistance)
            {
                Destroy(segments[i]);
                segments.RemoveAt(i);
            }
        }

        // Создаём новые
        if (segments.Count < poolSize)
        {
            CreateSegment(nextSpawnZ);
            nextSpawnZ += segmentLength;
        }
    }

    void CreateSegment(float zPosition)
    {
        GameObject newSegment = Instantiate(roadSegmentPrefab, new Vector3(0, -0.5f, zPosition), Quaternion.identity);
        newSegment.transform.parent = this.transform;
        segments.Add(newSegment);
    }
}
