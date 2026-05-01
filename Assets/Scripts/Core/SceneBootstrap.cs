using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ZorianyRaid.Player;
using ZorianyRaid.Enemies;
using ZorianyRaid.Spawning;
using ZorianyRaid.Pickups;
using ZorianyRaid.UI;
using ZorianyRaid.Scoring;
using ZorianyRaid.Background;

namespace ZorianyRaid.Core
{
    /// <summary>
    /// Стартова точка сцени. Будує всю ігрову ієрархію в Awake/Start
    /// (камера, гравець, префаби, спавнери, HUD, менеджери) — щоб
    /// проєкт запускався без ручного налаштування Inspector.
    /// </summary>
    public class SceneBootstrap : MonoBehaviour
    {
        [Header("Параметри гри")]
        [SerializeField] private bool useWaveManager = true;        // false → простий спавнер для етапу 1
        [SerializeField] private int playerMaxHealth = 3;

        // Кеш створених префабів
        private GameObject bulletPrefab;
        private GameObject dronePrefab;
        private GameObject asteroidPrefab;
        private GameObject powerUpPrefab;

        private void Awake()
        {
            SetupCamera();
            BuildPrefabs();
            BuildPlayer();
            BuildBackground();
            BuildManagers();
            BuildUI();
            BuildSpawners();
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.04f, 0.04f, 0.12f); // темно-синій космос
            cam.transform.position = new Vector3(0, 0, -10);
        }

        private void BuildPrefabs()
        {
            bulletPrefab = MakeBullet();
            dronePrefab = MakeDrone();
            asteroidPrefab = MakeAsteroid();
            powerUpPrefab = MakePowerUp();

            // Зробити префаби «латентними» — вони не на сцені, тільки шаблони
            bulletPrefab.SetActive(false);
            dronePrefab.SetActive(false);
            asteroidPrefab.SetActive(false);
            powerUpPrefab.SetActive(false);
            // ... але Instantiate активує копії
        }

        private GameObject MakeBullet()
        {
            GameObject go = new GameObject("Bullet");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateRect(new Color(1f, 0.95f, 0.5f), 8, 16);
            sr.sortingOrder = 5;
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.08f, 0.16f);

            go.AddComponent<Bullet>();
            return go;
        }

        private GameObject MakeDrone()
        {
            GameObject go = new GameObject("Drone");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateRect(new Color(0.95f, 0.25f, 0.25f), 32, 32);
            sr.sortingOrder = 4;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.32f, 0.32f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            go.AddComponent<Health>();
            go.AddComponent<DownwardMover>();
            var enemy = go.AddComponent<Enemy>();
            enemy.SetPowerUpPrefab(powerUpPrefab);

            return go;
        }

        private GameObject MakeAsteroid()
        {
            GameObject go = new GameObject("Asteroid");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateCircle(new Color(0.55f, 0.42f, 0.32f), 24);
            sr.sortingOrder = 4;
            go.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.12f;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            go.AddComponent<Health>();
            go.AddComponent<DownwardMover>();
            go.AddComponent<Asteroid>();
            return go;
        }

        private GameObject MakePowerUp()
        {
            GameObject go = new GameObject("PowerUp");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateCircle(new Color(0.3f, 1f, 0.4f), 24);
            sr.sortingOrder = 4;
            go.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.12f;

            go.AddComponent<DownwardMover>();
            go.AddComponent<PowerUp>();
            return go;
        }

        private void BuildPlayer()
        {
            GameObject go = new GameObject("Player");
            go.transform.position = new Vector3(0, -3.5f, 0);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.CreateTriangle(new Color(0.4f, 0.8f, 1f), 64);
            sr.sortingOrder = 6;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.5f, 0.6f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            var health = go.AddComponent<Health>();
            health.SetMax(playerMaxHealth, true);

            var pc = go.AddComponent<PlayerController>();

            // Точка пострілу
            GameObject firePoint = new GameObject("FirePoint");
            firePoint.transform.SetParent(go.transform);
            firePoint.transform.localPosition = new Vector3(0, 0.4f, 0);

            pc.Init(bulletPrefab, firePoint.transform);
        }

        private void BuildBackground()
        {
            // Без художніх ассетів робимо просто кілька «зірок»
            GameObject parent = new GameObject("Stars");
            for (int i = 0; i < 80; i++)
            {
                GameObject star = new GameObject($"Star_{i}");
                star.transform.SetParent(parent.transform);
                star.transform.position = new Vector3(
                    Random.Range(-9f, 9f),
                    Random.Range(-5f, 5f),
                    1f);
                var ssr = star.AddComponent<SpriteRenderer>();
                ssr.sprite = SpriteFactory.CreateRect(Color.white, 2, 2);
                ssr.sortingOrder = -10;
                star.transform.localScale = Vector3.one * Random.Range(0.5f, 1.2f);
                star.AddComponent<StarTwinkle>();
            }
        }

        private void BuildManagers()
        {
            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("ScoreManager").AddComponent<ScoreManager>();
        }

        private void BuildSpawners()
        {
            if (useWaveManager)
            {
                GameObject go = new GameObject("WaveManager");
                var wm = go.AddComponent<WaveManager>();
                wm.Init(dronePrefab, asteroidPrefab);
            }
            else
            {
                // Етап 1: простий спавнер дронів для тестування взаємодії
                GameObject go = new GameObject("SimpleSpawner");
                var sp = go.AddComponent<SimpleSpawner>();
                sp.Init(dronePrefab);
            }
        }

        private void BuildUI()
        {
            // Canvas
            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            // EventSystem (для кнопок)
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            UIManager ui = canvasGo.AddComponent<UIManager>();

            // HUD тексти
            TMP_Text scoreText = CreateText(canvasGo.transform, "Score: 0",
                new Vector2(1, 1), new Vector2(-160, -40), TextAlignmentOptions.Right);
            TMP_Text livesText = CreateText(canvasGo.transform, "HP: 3/3",
                new Vector2(0, 1), new Vector2(160, -40), TextAlignmentOptions.Left);
            TMP_Text waveText = CreateText(canvasGo.transform, "Wave 1",
                new Vector2(0.5f, 1), new Vector2(0, -40), TextAlignmentOptions.Center);

            // Банер хвилі по центру екрана
            GameObject banner = new GameObject("WaveBanner");
            banner.transform.SetParent(canvasGo.transform, false);
            TMP_Text bannerText = CreateText(banner.transform, "Хвиля 1",
                new Vector2(0.5f, 0.6f), Vector2.zero, TextAlignmentOptions.Center, 56);

            // Повідомлення
            TMP_Text messageText = CreateText(canvasGo.transform, "",
                new Vector2(0.5f, 0.4f), Vector2.zero, TextAlignmentOptions.Center, 32);

            // Game Over панель
            GameObject gameOverPanel = CreatePanel(canvasGo.transform, "GameOverPanel",
                new Color(0, 0, 0, 0.85f));
            TMP_Text titleText = CreateText(gameOverPanel.transform, "GAME OVER",
                new Vector2(0.5f, 0.7f), Vector2.zero, TextAlignmentOptions.Center, 64,
                new Color(1f, 0.3f, 0.3f));
            TMP_Text finalScore = CreateText(gameOverPanel.transform, "Ваш рахунок: 0",
                new Vector2(0.5f, 0.55f), Vector2.zero, TextAlignmentOptions.Center, 36);
            TMP_Text highScore = CreateText(gameOverPanel.transform, "Рекорд: 0",
                new Vector2(0.5f, 0.48f), Vector2.zero, TextAlignmentOptions.Center, 28);
            GameObject newHigh = CreateText(gameOverPanel.transform, "НОВИЙ РЕКОРД!",
                new Vector2(0.5f, 0.4f), Vector2.zero, TextAlignmentOptions.Center, 28,
                new Color(1f, 0.9f, 0.2f)).gameObject;
            CreateText(gameOverPanel.transform, "[R] — рестарт",
                new Vector2(0.5f, 0.25f), Vector2.zero, TextAlignmentOptions.Center, 24);
            gameOverPanel.SetActive(false);

            // Pause панель
            GameObject pausePanel = CreatePanel(canvasGo.transform, "PausePanel",
                new Color(0, 0, 0, 0.7f));
            CreateText(pausePanel.transform, "PAUSE\n[ESC/P] продовжити",
                new Vector2(0.5f, 0.5f), Vector2.zero, TextAlignmentOptions.Center, 48);
            pausePanel.SetActive(false);

            // Підключаємо все до UIManager напряму
            ui.Init(scoreText, waveText, livesText,
                    banner, bannerText, pausePanel,
                    gameOverPanel, finalScore, highScore, newHigh,
                    messageText);
        }

        private TMP_Text CreateText(Transform parent, string text,
            Vector2 anchor, Vector2 anchoredPos, TextAlignmentOptions align,
            float size = 28, Color? color = null)
        {
            GameObject go = new GameObject("Text_" + text);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(800, 80);
            rt.anchoredPosition = anchoredPos;

            TMP_Text t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.alignment = align;
            t.fontSize = size;
            t.color = color ?? Color.white;
            return t;
        }

        private GameObject CreatePanel(Transform parent, string name, Color bgColor)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = go.AddComponent<Image>();
            img.color = bgColor;
            return go;
        }
    }
}
