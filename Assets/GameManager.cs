using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startScreenUI; // Ссылка на нашу панель

    void Start()
    {
        // При старте сцены сразу ставим игру на паузу и показываем экран правил
        Time.timeScale = 0f;
        startScreenUI.SetActive(true);
    }

    // Эту функцию мы привяжем к кнопке
    public void StartGame()
    {
        // Прячем стартовый экран и запускаем время
        startScreenUI.SetActive(false);
        Time.timeScale = 1f;
    }
}