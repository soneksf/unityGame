using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZorianyRaid.Core;
using ZorianyRaid.Scoring;

namespace ZorianyRaid.UI
{
    /// <summary>
    /// Контролер ігрового HUD: бали, життя, поточна хвиля,
    /// банер «Хвиля X» при її початку, екрани Pause і Game Over.
    /// Підписується на події GameEvents — не тримає ніякої логіки.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text livesText;

        [Header("Банер хвилі")]
        [SerializeField] private GameObject waveBanner;
        [SerializeField] private TMP_Text waveBannerText;
        [SerializeField] private float bannerSeconds = 1.5f;

        [Header("Pause panel")]
        [SerializeField] private GameObject pausePanel;

        [Header("Game Over panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private GameObject newHighScoreLabel;

        [Header("Повідомлення")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float messageSeconds = 1.2f;

        private float bannerTimer;
        private float messageTimer;

        public void Init(TMP_Text score, TMP_Text wave, TMP_Text lives,
                         GameObject banner, TMP_Text bannerLabel,
                         GameObject pause, GameObject gameOver,
                         TMP_Text finalScore, TMP_Text high, GameObject newHigh,
                         TMP_Text msg)
        {
            scoreText = score;
            waveText = wave;
            livesText = lives;
            waveBanner = banner;
            waveBannerText = bannerLabel;
            pausePanel = pause;
            gameOverPanel = gameOver;
            finalScoreText = finalScore;
            highScoreText = high;
            newHighScoreLabel = newHigh;
            messageText = msg;
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.PlayerHealthChanged += OnHealthChanged;
            GameEvents.WaveStarted += OnWaveStarted;
            GameEvents.WaveCompleted += OnWaveCompleted;
            GameEvents.GamePaused += OnPaused;
            GameEvents.GameResumed += OnResumed;
            GameEvents.GameOver += OnGameOver;
            GameEvents.NewHighScore += OnNewHighScore;
            GameEvents.PowerUpCollected += OnPowerUpCollected;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.PlayerHealthChanged -= OnHealthChanged;
            GameEvents.WaveStarted -= OnWaveStarted;
            GameEvents.WaveCompleted -= OnWaveCompleted;
            GameEvents.GamePaused -= OnPaused;
            GameEvents.GameResumed -= OnResumed;
            GameEvents.GameOver -= OnGameOver;
            GameEvents.NewHighScore -= OnNewHighScore;
            GameEvents.PowerUpCollected -= OnPowerUpCollected;
        }

        private void Start()
        {
            if (waveBanner != null) waveBanner.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (newHighScoreLabel != null) newHighScoreLabel.SetActive(false);
            if (messageText != null) messageText.text = string.Empty;
        }

        private void Update()
        {
            // Не використовуємо WaitForSeconds, щоб не плодити корутин
            if (bannerTimer > 0f)
            {
                bannerTimer -= Time.unscaledDeltaTime;
                if (bannerTimer <= 0f && waveBanner != null) waveBanner.SetActive(false);
            }

            if (messageTimer > 0f)
            {
                messageTimer -= Time.unscaledDeltaTime;
                if (messageTimer <= 0f && messageText != null) messageText.text = string.Empty;
            }
        }

        // --- Слухачі подій ---

        private void OnScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        private void OnHealthChanged(int cur, int max)
        {
            if (livesText != null) livesText.text = $"HP: {cur}/{max}";
        }

        private void OnWaveStarted(int wave)
        {
            if (waveText != null) waveText.text = $"Wave {wave}";
            if (waveBanner != null && waveBannerText != null)
            {
                waveBannerText.text = $"Хвиля {wave}";
                waveBanner.SetActive(true);
                bannerTimer = bannerSeconds;
            }
        }

        private void OnWaveCompleted(int wave)
        {
            ShowMessage($"Хвилю {wave} завершено! +{wave * 200}");
        }

        private void OnPaused()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        private void OnResumed()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void OnGameOver()
        {
            if (gameOverPanel == null) return;
            gameOverPanel.SetActive(true);

            int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
            int high = ScoreManager.Instance != null ? ScoreManager.Instance.HighScore : 0;

            if (finalScoreText != null) finalScoreText.text = $"Ваш рахунок: {finalScore}";
            if (highScoreText != null) highScoreText.text = $"Рекорд: {Mathf.Max(high, finalScore)}";
        }

        private void OnNewHighScore()
        {
            if (newHighScoreLabel != null) newHighScoreLabel.SetActive(true);
            ShowMessage("Новий рекорд!");
        }

        private void OnPowerUpCollected()
        {
            ShowMessage("+1 HP");
        }

        private void ShowMessage(string text)
        {
            if (messageText == null) return;
            messageText.text = text;
            messageTimer = messageSeconds;
        }

        // --- Кнопки UI ---
        public void OnResumeButton() => GameManager.Instance?.Resume();
        public void OnRestartButton() => GameManager.Instance?.Restart();
        public void OnQuitButton() => GameManager.Instance?.QuitToMenu();
    }
}
