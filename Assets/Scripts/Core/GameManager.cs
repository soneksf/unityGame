using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZorianyRaid.Core
{
    /// <summary>
    /// Сінглтон, що тримає глобальний стан гри: Playing / Paused / GameOver.
    /// Реагує на смерть гравця, керує паузою, рестартом і виходом у меню.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum GameState { Playing, Paused, GameOver }

        public static GameManager Instance { get; private set; }
        public GameState State { get; private set; } = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.PlayerDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.PlayerDied -= HandlePlayerDied;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            State = GameState.Playing;
            GameEvents.RaiseGameStarted();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (State == GameState.Playing) Pause();
                else if (State == GameState.Paused) Resume();
            }

            if (State == GameState.GameOver && Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }

        public void Pause()
        {
            if (State != GameState.Playing) return;
            State = GameState.Paused;
            Time.timeScale = 0f;
            GameEvents.RaiseGamePaused();
        }

        public void Resume()
        {
            if (State != GameState.Paused) return;
            State = GameState.Playing;
            Time.timeScale = 1f;
            GameEvents.RaiseGameResumed();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            // Якщо буде окрема сцена меню — підставити її ім'я тут
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandlePlayerDied()
        {
            State = GameState.GameOver;
            Time.timeScale = 0f;
            GameEvents.RaiseGameOver();
        }
    }
}
