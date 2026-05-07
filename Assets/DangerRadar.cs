using UnityEngine;

public class DangerRadar : MonoBehaviour
{
    public GameObject dangerUI; 
    private int enemiesInZone = 0; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInZone++;
            if (dangerUI != null) dangerUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInZone--;
            if (enemiesInZone <= 0)
            {
                enemiesInZone = 0;
                if (dangerUI != null) dangerUI.SetActive(false); 
            }
        }
    }
}
