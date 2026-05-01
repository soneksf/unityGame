using System;

namespace ZorianyRaid.Core
{
    /// <summary>
    /// Глобальна шина подій для слабкого зв'язування між системами.
    /// UI та менеджери підписуються на події, ігрові об'єкти їх кидають.
    /// </summary>
    public static class GameEvents
    {
        // Гравець
        public static event Action<int, int> PlayerHealthChanged; // current, max
        public static event Action PlayerDied;

        // Очки і прогрес
        public static event Action<int> ScoreChanged;
        public static event Action<int> WaveStarted;
        public static event Action<int> WaveCompleted;
        public static event Action NewHighScore;

        // Ігровий стан
        public static event Action GameStarted;
        public static event Action GamePaused;
        public static event Action GameResumed;
        public static event Action GameOver;

        // Ігрові об'єкти
        public static event Action<int> EnemyDestroyed; // нагорода в очках
        public static event Action<int> AsteroidDestroyed;
        public static event Action PowerUpCollected;
        public static event Action<int> ScoreAwarded; // довільна нагорода (наприклад, бонус за хвилю)

        // Інвокатори (раздільно, щоб не плутати з Action?.Invoke у викликаючих)
        public static void RaisePlayerHealthChanged(int current, int max) => PlayerHealthChanged?.Invoke(current, max);
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
        public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
        public static void RaiseWaveStarted(int wave) => WaveStarted?.Invoke(wave);
        public static void RaiseWaveCompleted(int wave) => WaveCompleted?.Invoke(wave);
        public static void RaiseNewHighScore() => NewHighScore?.Invoke();
        public static void RaiseGameStarted() => GameStarted?.Invoke();
        public static void RaiseGamePaused() => GamePaused?.Invoke();
        public static void RaiseGameResumed() => GameResumed?.Invoke();
        public static void RaiseGameOver() => GameOver?.Invoke();
        public static void RaiseEnemyDestroyed(int reward) => EnemyDestroyed?.Invoke(reward);
        public static void RaiseAsteroidDestroyed(int reward) => AsteroidDestroyed?.Invoke(reward);
        public static void RaisePowerUpCollected() => PowerUpCollected?.Invoke();
        public static void RaiseScoreAwarded(int amount) => ScoreAwarded?.Invoke(amount);
    }
}
