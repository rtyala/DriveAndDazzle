using UnityEngine;
using System.Collections.Generic;

public class HouseGenerator : MonoBehaviour
{
    [Header("Настройки домов")]
    public float houseSpacing = 3f;        // Мин. расстояние между краями соседних домов
    public float leftOffset = -14.5f;      // Слева от дороги
    public float rightOffset = 14.5f;      // Справа от дороги
    public float minSize = 1f;             // Минимальная ширина (X)
    public float maxSize = 1.5f;           // Максимальная ширина (X)
    public float minHeight = 1.5f;         // Минимальная высота
    public float maxHeight = 20f;          // Максимальная высота

    public int maxAttemptsPerHouse = 100;  // Попыток найти свободное место
    public int maxHousesPerSide = 30;      // Максимум домов с каждой стороны

    public Color[] colors = new Color[]
    {
        new Color(1f, 0f, 0f),       // Красный
        new Color(1f, 0.5f, 0f),     // Оранжевый
        new Color(1f, 1f, 0f),       // Жёлтый
        new Color(0f, 1f, 0f),       // Зелёный
        new Color(0f, 1f, 1f),       // Голубой
        new Color(0f, 0f, 1f),       // Синий
        new Color(0.5f, 0f, 0.8f)    // Фиолетовый
    };

    private float segmentLength;
    private bool hasGenerated = false;

    // Храним данные уже расставленных домов для проверки коллизий
    private List<float> leftHouseZ = new List<float>();
    private List<float> leftHouseDepth = new List<float>();
    private List<float> rightHouseZ = new List<float>();
    private List<float> rightHouseDepth = new List<float>();

    void Start()
    {
        segmentLength = 20f;
        GenerateHouses();
    }

    void GenerateHouses()
    {
        if (hasGenerated) return;

        ClearOldHouses();
        leftHouseZ.Clear();
        leftHouseDepth.Clear();
        rightHouseZ.Clear();
        rightHouseDepth.Clear();

        // Границы области, где могут находиться центры домов (с учётом половинной глубины и отступа)
        float maxHalfDepth = 2f / 2f;     // максимальная глубина 2 -> половина = 1
        float minZ = -segmentLength / 2f + maxHalfDepth + 0.5f;
        float maxZ = segmentLength / 2f - maxHalfDepth - 0.5f;

        // Генерируем дома слева
        PlaceHousesOnSide(leftOffset, minZ, maxZ, leftHouseZ, leftHouseDepth);

        // Генерируем дома справа
        PlaceHousesOnSide(rightOffset, minZ, maxZ, rightHouseZ, rightHouseDepth);

        hasGenerated = true;
    }

    void PlaceHousesOnSide(float x, float minZ, float maxZ,
                           List<float> zList, List<float> depthList)
    {
        for (int i = 0; i < maxHousesPerSide; i++)
        {
            bool placed = false;
            for (int attempt = 0; attempt < maxAttemptsPerHouse; attempt++)
            {
                // Случайная позиция центра по Z
                float z = Random.Range(minZ, maxZ);
                float depth = Random.Range(1f, 2f);   // глубина дома

                // Проверяем, не нарушает ли новый дом зазор houseSpacing
                if (!OverlapsWithExisting(z, depth, zList, depthList, houseSpacing))
                {
                    // Место свободно — создаём дом
                    CreateHouse(x, z, depth);
                    zList.Add(z);
                    depthList.Add(depth);
                    placed = true;
                    break;
                }
            }

            // Если за все попытки не нашли места — прекращаем ставить дома на этой стороне
            if (!placed)
                break;
        }
    }

    bool OverlapsWithExisting(float newZ, float newDepth,
                              List<float> existingZ, List<float> existingDepth,
                              float minGap)
    {
        float newHalf = newDepth * 0.5f;
        for (int i = 0; i < existingZ.Count; i++)
        {
            float existHalf = existingDepth[i] * 0.5f;
            // Расстояние между центрами должно быть >= сумме полуглубин + минимальный зазор
            if (Mathf.Abs(newZ - existingZ[i]) < newHalf + existHalf + minGap)
                return true;   // пересечение или слишком близко
        }
        return false;
    }

    void CreateHouse(float x, float z, float depth)
    {
        float width = Random.Range(minSize, maxSize);
        float height = Random.Range(minHeight, maxHeight);
        Color houseColor = colors[Random.Range(0, colors.Length)];

        GameObject houseBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        houseBody.name = $"House_{(x < 0 ? "Left" : "Right")}_{z:F1}";
        houseBody.transform.parent = this.transform;
        houseBody.transform.localPosition = new Vector3(x, height / 2-0.5f, z);
        houseBody.transform.localScale = new Vector3(width, height, depth);

        Material bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        bodyMat.color = houseColor;
        houseBody.GetComponent<Renderer>().material = bodyMat;
    }

    void ClearOldHouses()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("House_"))
                DestroyImmediate(child.gameObject);
        }
    }
}