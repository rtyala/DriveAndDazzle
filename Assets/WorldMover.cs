using UnityEngine;

public class WorldMover : MonoBehaviour
{
    [SerializeField] private float worldSpeed = 0.03f;

    void Update()
    {
        transform.Translate(Vector3.back * worldSpeed * Time.deltaTime);
    }

    public float GetWorldSpeed() => worldSpeed;
}