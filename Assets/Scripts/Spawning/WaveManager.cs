using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZorianyRaid.Core;
using ZorianyRaid.Enemies;

namespace ZorianyRaid.Spawning
{
    /// <summary>
    /// Менеджер хвиль — основа ігрового циклу.
    /// Генерує дронів та астероїди за формулами з ГДД,
    /// відстежує очищення сцени і запускає наступну хвилю.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [Header("Префаби")]
        [SerializeField] private GameObject dronePrefab;
        [SerializeField] private GameObject asteroidPrefab;

        [Header("Зона спавна (верх екрана)")]
        [SerializeField] private float spawnY = 6f;
        [SerializeField] private float spawnXMin = -7f;
        [SerializeField] private float spawnXMax = 7f;

        [Header("Базові параметри")]
        [SerializeField] private int baseEnemyCount = 3;
        [SerializeField] private float baseEnemySpeed = 2f;
        [SerializeField] private float baseAsteroidSpeed = 1.5f;
        [SerializeField] private float baseSpawnInterval = 0.8f;
        [SerializeField] private float pauseBetweenWaves = 2f;

        [Header("Введення астероїдів і темпу")]
        [SerializeField] private int asteroidsAppearAtWave = 4;
        [SerializeField] private int hardModeWave = 8;

        public int CurrentWave { get; private set; }
        public int AliveEnemies => activeSpawned.Count;

        public void Init(GameObject drone, GameObject asteroid)
        {
            this.dronePrefab = drone;
            this.asteroidPrefab = asteroid;
        }

        private readonly List<GameObject> activeSpawned = new List<GameObject>();
        private bool spawningPhase;

        private void Start()
        {
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            while (true)
            {
                CurrentWave++;
                GameEvents.RaiseWaveStarted(CurrentWave);

                yield return SpawnWaveRoutine(CurrentWave);

                // Дочекатись, поки гравець очистить хвилю
                while (activeSpawned.Count > 0)
                {
                    activeSpawned.RemoveAll(o => o == null);
                    yield return null;
                }

                GameEvents.RaiseWaveCompleted(CurrentWave);
                yield return new WaitForSeconds(pauseBetweenWaves);
            }
        }

        private IEnumerator SpawnWaveRoutine(int wave)
        {
            spawningPhase = true;

            int enemyCount = Mathf.RoundToInt(baseEnemyCount + wave * 0.5f);
            int asteroidCount = (wave >= asteroidsAppearAtWave)
                ? Mathf.RoundToInt(2 + (wave - asteroidsAppearAtWave) * 0.5f)
                : 0;
            if (wave >= hardModeWave) enemyCount = Mathf.Min(12, enemyCount + 2);

            float enemySpeed = baseEnemySpeed * (1f + wave * 0.03f);
            float asteroidSpeed = baseAsteroidSpeed * (1f + wave * 0.05f);
            float interval = Mathf.Max(0.3f, baseSpawnInterval - wave * 0.04f);

            int total = enemyCount + asteroidCount;
            for (int i = 0; i < total; i++)
            {
                bool spawnAsteroid = (i >= enemyCount);
                if (spawnAsteroid) SpawnAsteroid(asteroidSpeed);
                else SpawnDrone(enemySpeed);

                yield return new WaitForSeconds(interval);
            }

            spawningPhase = false;
        }

        private void SpawnDrone(float speed)
        {
            if (dronePrefab == null) return;
            Vector3 pos = new Vector3(Random.Range(spawnXMin, spawnXMax), spawnY, 0f);
            GameObject go = Instantiate(dronePrefab, pos, Quaternion.identity);
            go.SetActive(true);
            ConfigureSpeed(go, speed);
            activeSpawned.Add(go);
        }

        private void SpawnAsteroid(float speed)
        {
            if (asteroidPrefab == null) return;
            Vector3 pos = new Vector3(Random.Range(spawnXMin, spawnXMax), spawnY, 0f);
            GameObject go = Instantiate(asteroidPrefab, pos, Quaternion.identity);
            go.SetActive(true);
            // Розкид швидкості 80–120% від базової
            ConfigureSpeed(go, speed * Random.Range(0.8f, 1.2f));
            activeSpawned.Add(go);
        }

        private void ConfigureSpeed(GameObject go, float speed)
        {
            var mover = go.GetComponent<DownwardMover>();
            if (mover != null) mover.Speed = speed;
        }
    }
}
