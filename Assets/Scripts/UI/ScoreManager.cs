using UnityEngine;
using ZorianyRaid.Core;

namespace ZorianyRaid.Scoring
{
    /// <summary>
    /// Сінглтон підрахунку балів. Слухає події знищення ворогів,
    /// астероїдів і прямого нарахування. Зберігає рекорд через PlayerPrefs
    /// і піднімає подію NewHighScore при оновленні.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private const string HighScoreKey = "ZR_HighScore";

        public int Score { get; private set; }
        public int HighScore { get; private set; }

        private bool highScoreAnnouncedThisRun;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void OnEnable()
        {
            GameEvents.EnemyDestroyed += AddScore;
            GameEvents.AsteroidDestroyed += AddScore;
            GameEvents.ScoreAwarded += AddScore;
            GameEvents.WaveCompleted += OnWaveCompleted;
            GameEvents.GameOver += SaveHighScore;
        }

        private void OnDisable()
        {
            GameEvents.EnemyDestroyed -= AddScore;
            GameEvents.AsteroidDestroyed -= AddScore;
            GameEvents.ScoreAwarded -= AddScore;
            GameEvents.WaveCompleted -= OnWaveCompleted;
            GameEvents.GameOver -= SaveHighScore;
        }

        private void Start()
        {
            GameEvents.RaiseScoreChanged(Score);
        }

        private void AddScore(int amount)
        {
            if (amount == 0) return;
            Score += amount;
            GameEvents.RaiseScoreChanged(Score);

            if (!highScoreAnnouncedThisRun && Score > HighScore)
            {
                highScoreAnnouncedThisRun = true;
                GameEvents.RaiseNewHighScore();
            }
        }

        private void OnWaveCompleted(int waveNumber)
        {
            int bonus = waveNumber * 200;
            AddScore(bonus);
        }

        private void SaveHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
                PlayerPrefs.Save();
            }
        }
    }
}
