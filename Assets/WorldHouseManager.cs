using UnityEngine;
using System.Collections.Generic;

public class WorldHouseManager : MonoBehaviour
{
    [Header("Префабы из ассета")]
    public GameObject[] buildingPrefabs;
    public GameObject[] treePrefabs;

    [Header("Настройки генерации домов")]
    public float leftX = -34f;
    public float rightX = 34f;
    public float gapBetweenObjects = 5f;

    [Header("Частота появления")]
    [Range(0f, 1f)] public float treeChance = 0.4f;

    [Header("Динамическая генерация")]
    public float generationAheadDistance = 100f;
    public float deleteBehindDistance = 80f;
    public float worldSpeed = 30f;

    private List<GameObject> activeObjects = new List<GameObject>();
    private float nextGenerateZ = 0f;
    private float currentWorldZ = 0f;

    void Start()
    {
        nextGenerateZ = 0f;
        currentWorldZ = 0f;
    }

    void Update()
    {
        // Мир движется
        currentWorldZ += Time.deltaTime * worldSpeed;

        // Двигаем все объекты назад
        for (int i = 0; i < activeObjects.Count; i++)
        {
            if (activeObjects[i] != null)
            {
                Vector3 pos = activeObjects[i].transform.position;
                pos.z -= Time.deltaTime * worldSpeed;
                activeObjects[i].transform.position = pos;
            }
        }

        // Генерируем новые объекты ВПЕРЕДИ по движению мира
        while (nextGenerateZ < currentWorldZ + generationAheadDistance)
        {
            CreateObjectPair(nextGenerateZ);
            nextGenerateZ += gapBetweenObjects;
        }

        // Удаляем объекты ПОЗАДИ (используем position.z объектов, а не currentWorldZ)
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null)
            {
                activeObjects.RemoveAt(i);
            }
            else if (activeObjects[i].transform.position.z < -deleteBehindDistance)
            {
                Destroy(activeObjects[i]);
                activeObjects.RemoveAt(i);
            }
        }
    }

    void CreateObjectPair(float z)
    {
        // Левая сторона
        GameObject prefabLeft = GetRandomPrefab();
        if (prefabLeft != null)
        {
            GameObject objLeft = Instantiate(prefabLeft);
            objLeft.transform.position = new Vector3(leftX, -0.5f, z);
            activeObjects.Add(objLeft);
        }

        // Правая сторона
        GameObject prefabRight = GetRandomPrefab();
        if (prefabRight != null)
        {
            GameObject objRight = Instantiate(prefabRight);
            objRight.transform.position = new Vector3(rightX, -0.5f, z);
            activeObjects.Add(objRight);
        }
    }

    GameObject GetRandomPrefab()
    {
        if (treePrefabs != null && treePrefabs.Length > 0 && Random.value < treeChance)
        {
            return treePrefabs[Random.Range(0, treePrefabs.Length)];
        }
        else if (buildingPrefabs != null && buildingPrefabs.Length > 0)
        {
            return buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        }

        return null;
    }
}