using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Если объект, который в нас въехал, имеет тег Enemy
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("АВАРИЯ!");
            Time.timeScale = 0f; // Останавливаем время
        }
    }
}