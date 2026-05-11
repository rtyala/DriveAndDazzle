using UnityEngine;
using System.Collections.Generic;

public class EndlessRoadGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject roadSegmentPrefab;

    [Header("Настройки")]
    public float segmentLength = 200f;
    public int segmentsInReserve = 5;
    public float deleteThresholdZ = -150f;

    private List<GameObject> segments = new List<GameObject>();
    private float nextLocalZ;

    void Start()
    {
        // ЧТОБЫ КАМЕРА ВИДЕЛА ДОРОГУ СРАЗУ:
        // Начинаем спавн с Z = -200 (или -segmentLength), 
        // чтобы первый сегмент уже лежал под камерой и впереди неё.
        nextLocalZ = -segmentLength + (segmentLength / 2f);

        for (int i = 0; i < segmentsInReserve; i++)
        {
            CreateSegment();
        }
    }

    void Update()
    {
        // 1. Удаление
        if (segments.Count > 0)
        {
            if (segments[0].transform.position.z < deleteThresholdZ)
            {
                GameObject old = segments[0];
                segments.RemoveAt(0);
                Destroy(old);
            }
        }

        // 2. Восполнение запаса
        while (segments.Count < segmentsInReserve)
        {
            CreateSegment();
        }
    }

    void CreateSegment()
    {
        if (roadSegmentPrefab == null) return;

        GameObject newSegment = Instantiate(roadSegmentPrefab);
        newSegment.transform.SetParent(this.transform.parent);

        // Устанавливаем локальную позицию
        newSegment.transform.localPosition = new Vector3(0, -0.5f, nextLocalZ);

        newSegment.transform.localScale = roadSegmentPrefab.transform.localScale;
        newSegment.transform.localRotation = Quaternion.identity;

        segments.Add(newSegment);

        nextLocalZ += segmentLength;
    }
}