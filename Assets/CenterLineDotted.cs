using UnityEngine;

public class CenterLineGenerator : MonoBehaviour
{
    public float dashLength = 0.4f;
    public float dashGap = 0.4f;
    public float lineWidth = 0.2f;
    public float lineHeight = 0.05f;

    void Start()
    {
        GenerateLines();
    }

    void GenerateLines()
    {
        // Очищаем старые линии
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        float startZ = -5f;
        float endZ = 5f;
        float currentZ = startZ;

        while (currentZ < endZ)
        {
            GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dash.transform.parent = transform;
            dash.transform.localPosition = new Vector3(0, lineHeight / 2f, currentZ);
            dash.transform.localScale = new Vector3(lineWidth, lineHeight, dashLength);

            // Назначаем белый цвет
            Renderer rend = dash.GetComponent<Renderer>();
            rend.material.color = Color.white;

            // Удаляем коллайдер — он не нужен для линий разметки
            Destroy(dash.GetComponent<Collider>());

            currentZ += dashLength + dashGap;
        }
    }
}