using UnityEngine;

[ExecuteAlways]
public class CenterLineGenerator : MonoBehaviour
{
    public float dashLength = 0.4f;
    public float dashGap = 0.4f;

    void Start()
    {
        GenerateLines();
    }

    void GenerateLines()
    {
        // Очищаем старые линии
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        float startZ = -5f;
        float endZ = 5f;
        float currentZ = startZ;

        while (currentZ < endZ)
        {
            GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dash.transform.parent = transform;
            dash.transform.localPosition = new Vector3(0, 0.1f, currentZ);
            dash.transform.localScale = new Vector3(0.2f, 0.05f, dashLength);

            // Белый цвет
            dash.GetComponent<Renderer>().material.color = Color.white;

            currentZ += dashLength + dashGap;
        }
    }
}