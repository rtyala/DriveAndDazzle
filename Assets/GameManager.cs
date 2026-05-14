using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startScreenUI; // Ссылка на нашу панель

    public AudioSource lobbySource;
    public AudioSource backgroundSource;
    public AudioSource radioSource;

    public AudioClip[] radioPlaylist;
    private int currentTrackIndex = 0;

    void Start()
    {
        // При старте сцены сразу ставим игру на паузу и показываем экран правил
        Time.timeScale = 0f;
        startScreenUI.SetActive(true);
        if (lobbySource != null) lobbySource.Play();
    }

    // Эту функцию мы привяжем к кнопке
    public void StartGame()
    {
        // Прячем стартовый экран и запускаем время
        startScreenUI.SetActive(false);
        Time.timeScale = 1f;
        if (lobbySource != null)
            lobbySource.Stop();
        if (backgroundSource != null)
            backgroundSource.Play();
    }

    public void ToggleRadio()
    {
        if (radioSource.isPlaying)
        {
            radioSource.Stop();
            backgroundSource.UnPause();
        }
        else
        {
            backgroundSource.Pause();
            radioSource.clip = radioPlaylist[currentTrackIndex];
            radioSource.Play();
            currentTrackIndex = (currentTrackIndex + 1) % radioPlaylist.Length;
        }
    }
}