using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Стартовый экран")]
    public GameObject startScreenUI;

    [Header("Финальный экран")]
    public GameObject gameOverScreenUI;

    [Header("Экран аварии")]
    public GameObject gameFastOverScreenUI;

    [Header("Настройки времени")]
    public float timeLimit = 600f;
    public static bool isGameOver = false;

    [Header("Текстовые компоненты")]
    public Text timerTextLegacy;
    public TMP_Text timerTextTMP;

    [Header("Аудио настройки")]
    public AudioSource lobbySource;
    public AudioSource backgroundSource;
    public AudioSource radioSource;

    public AudioClip[] radioPlaylist;
    private int currentTrackIndex = 0;

    [Header("Ссылки для Результатов")]
    public TexturePainter texturePainter;
    public RawImage finalHandsImage;
    public Image diagramFillImage;
    public TMP_Text percentTextUI;

    [Header("Элементы Финала и Свиданий")]
    [Tooltip("Порог процентов для победы (например, 70)")]
    public float requiredSuccessPercent = 70f;
    public Button goToDateButton; // Вторая кнопка ("На свидание")
    public TMP_Text winLoseText; // Текст "Ура вы выиграли" / "Увы вы не успели"

    [Header("Экран Свидания (Новая логика)")]
    public GameObject dateScreenUI; // Экран свидания, который включается кнопкой
    public Image finalDateCandidateImage; // Компонент Image на экране свидания, куда встанет мужичок
    public Sprite[] manSprites; // Массив из 5 картинок красивых мужчин

    void Start()
    {
        if (gameFastOverScreenUI != null)
            gameFastOverScreenUI.SetActive(false);

        isGameOver = false;

        Time.timeScale = 0f;
        startScreenUI.SetActive(true);

        if (gameOverScreenUI != null)
            gameOverScreenUI.SetActive(false);

        if (dateScreenUI != null)
            dateScreenUI.SetActive(false);

        // Изначально кнопка свидания отключена и текст пустой
        if (goToDateButton != null) goToDateButton.interactable = false;
        if (winLoseText != null) winLoseText.text = "";

        if (lobbySource != null)
            lobbySource.Play();
    }

    public void StartGame()
    {
        startScreenUI.SetActive(false);
        Time.timeScale = 1f;

        if (lobbySource != null)
            lobbySource.Stop();
        if (backgroundSource != null)
            backgroundSource.Play();
    }

    public void ToggleRadio()
    {
        if (isGameOver) return;

        if (radioSource.isPlaying)
        {
            radioSource.Stop();
            if (backgroundSource != null)
                backgroundSource.UnPause();
        }
        else
        {
            if (backgroundSource != null)
                backgroundSource.Pause();

            if (radioPlaylist != null && radioPlaylist.Length > 0)
            {
                radioSource.clip = radioPlaylist[currentTrackIndex];
                radioSource.Play();
                currentTrackIndex = (currentTrackIndex + 1) % radioPlaylist.Length;
            }
        }
    }

    void Update()
    {
        if (isGameOver || Time.timeScale == 0f) return;

        timeLimit -= Time.deltaTime;
        UpdateTimerUI();

        if (timeLimit <= 0)
        {
            timeLimit = 0;
            FinishGame();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLimit / 60F);
        int seconds = Mathf.FloorToInt(timeLimit - minutes * 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

        SetTimerText(timerString);
    }

    void SetTimerText(string text)
    {
        if (timerTextLegacy != null)
            timerTextLegacy.text = text;
        else if (timerTextTMP != null)
            timerTextTMP.text = text;
    }

    void FinishGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (backgroundSource != null && backgroundSource.isPlaying)
            backgroundSource.Stop();
        if (radioSource != null && radioSource.isPlaying)
            radioSource.Stop();
        if (lobbySource != null && lobbySource.isPlaying)
            lobbySource.Stop();

        HideTimerText();

        if (texturePainter != null && finalHandsImage != null)
        {
            texturePainter.TransferTextureToFinalScreen(finalHandsImage);
        }

        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(true);
            float finalAccuracy = 0f;
            if (texturePainter != null)
            {
                finalAccuracy = texturePainter.GetSuccessPercentage();
            }
            StartCoroutine(AnimateDiagram(finalAccuracy));
        }
    }

    private IEnumerator AnimateDiagram(float targetPercent)
    {
        targetPercent = Mathf.Clamp(targetPercent, 0f, 100f);
        if (diagramFillImage != null) diagramFillImage.fillAmount = 0f;
        if (percentTextUI != null) percentTextUI.text = "0%";

        float currentPercent = 0f;
        float animationSpeed = 50f;

        while (currentPercent < targetPercent)
        {
            currentPercent += animationSpeed * Time.unscaledDeltaTime;

            if (currentPercent > targetPercent)
                currentPercent = targetPercent;

            if (diagramFillImage != null)
                diagramFillImage.fillAmount = currentPercent / 100f;

            if (percentTextUI != null)
                percentTextUI.text = Mathf.RoundToInt(currentPercent).ToString() + "%";

            yield return null;
        }

        CheckWinLoseConditions(targetPercent);
    }

    private void CheckWinLoseConditions(float finalPercent)
    {
        if (finalPercent >= requiredSuccessPercent)
        {
            if (winLoseText != null) winLoseText.text = "Ура вы выиграли";
            if (goToDateButton != null) goToDateButton.interactable = true;
        }
        else
        {
            if (winLoseText != null) winLoseText.text = "Увы вы не успели";
            if (goToDateButton != null) goToDateButton.interactable = false;
        }
    }

    // МЕТОД ДЛЯ КНОПКИ "НА СВИДАНИЕ" (Выбирает мужичка сам!)
    public void OpenDateScreen()
    {
        if (manSprites != null && manSprites.Length > 0 && finalDateCandidateImage != null)
        {
            // Сама программа выбирает случайный индекс от 0 до длины массива
            int randomIndex = Random.Range(0, manSprites.Length);

            // Подставляем выбранную картинку на экран свидания
            finalDateCandidateImage.sprite = manSprites[randomIndex];
        }

        // Выключаем экран результатов, включаем экран свидания
        if (gameOverScreenUI != null) gameOverScreenUI.SetActive(false);
        if (dateScreenUI != null) dateScreenUI.SetActive(true);
    }

    void HideTimerText()
    {
        if (timerTextLegacy != null && timerTextLegacy.gameObject != null)
            timerTextLegacy.gameObject.SetActive(false);
        else if (timerTextTMP != null && timerTextTMP.gameObject != null)
            timerTextTMP.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        FinishGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FastGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        if (backgroundSource != null) backgroundSource.Stop();
        if (radioSource != null) radioSource.Stop();

        HideTimerText();

        if (gameFastOverScreenUI != null)
        {
            gameFastOverScreenUI.SetActive(true);
        }
    }
}