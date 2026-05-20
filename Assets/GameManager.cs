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

    public TexturePainter texturePainter;
    public RawImage finalHandsImage;
    public Image diagramFillImage;
    public TMP_Text percentTextUI;


    void Start()
    {
        if (gameFastOverScreenUI != null) 
            gameFastOverScreenUI.SetActive(false);

        isGameOver = false;

        Time.timeScale = 0f;
        startScreenUI.SetActive(true);

        if (gameOverScreenUI != null)
            gameOverScreenUI.SetActive(false);

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
        {
            timerTextLegacy.text = text;
        }
        else if (timerTextTMP != null)
        {
            timerTextTMP.text = text;
        }
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
            {
                diagramFillImage.fillAmount = currentPercent / 100f;
            }

            if (percentTextUI != null)
            {
                percentTextUI.text = Mathf.RoundToInt(currentPercent).ToString() + "%";
            }

            yield return null;
        }
    }

    void HideTimerText()
    {
        if (timerTextLegacy != null && timerTextLegacy.gameObject != null)
        {
            timerTextLegacy.gameObject.SetActive(false);
        }
        else if (timerTextTMP != null && timerTextTMP.gameObject != null)
        {
            timerTextTMP.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (backgroundSource != null && backgroundSource.isPlaying) backgroundSource.Stop();
        if (radioSource != null && radioSource.isPlaying) radioSource.Stop();

        HideTimerText();

        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(true);

            float currentAccuracy = 0f;
            if (texturePainter != null)
            {
                texturePainter.TransferTextureToFinalScreen(finalHandsImage);
                currentAccuracy = texturePainter.GetSuccessPercentage();
            }
            StartCoroutine(AnimateDiagram(currentAccuracy));
        }
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