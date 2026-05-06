using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 20f;
    public float zDespawnLimit = -30f;

    void Update()
    {
        // Vector3.back - это вектор (0, 0, -1). Он заставляет Z уменьшаться.
        // Space.World гарантирует, что машина поедет навстречу камере, 
        // даже если сама 3D-модель повернута боком.
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.z < zDespawnLimit)
        {
            gameObject.SetActive(false);
        }
    }
}