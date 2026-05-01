using UnityEngine;
using ZorianyRaid.Core;

namespace ZorianyRaid.Player
{
    /// <summary>
    /// Контролер гравця: рух WASD/стрілки, обмеження областю екрана,
    /// автоматична стрільба, обробка зіткнень з ворогами/астероїдами,
    /// тимчасова невразливість після уроку.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Рух")]
        [SerializeField] private float moveSpeed = 6f;

        [Header("Стрільба")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.25f;

        [Header("Невразливість")]
        [SerializeField] private float invincibilitySeconds = 2f;
        [SerializeField] private float blinkRate = 12f;

        [Header("Межі сцени (нижня третина екрана)")]
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;
        [SerializeField] private float minY = -4.5f;
        [SerializeField] private float maxY = -1f;

        private Health health;
        private SpriteRenderer sr;
        private Rigidbody2D rb;
        private float fireTimer;
        private float invincibilityTimer;

        private void Awake()
        {
            health = GetComponent<Health>();
            sr = GetComponentInChildren<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        private void OnEnable()
        {
            health.Changed += OnHealthChanged;
            health.Died += OnPlayerDied;
        }

        private void OnDisable()
        {
            health.Changed -= OnHealthChanged;
            health.Died -= OnPlayerDied;
        }

        private void Start()
        {
            // Початкова синхронізація HUD
            GameEvents.RaisePlayerHealthChanged(health.Current, health.Max);
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
                return;

            HandleMovement();
            HandleShooting();
            HandleInvincibility();
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 dir = new Vector2(h, v);
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            Vector2 delta = dir * moveSpeed * Time.deltaTime;
            Vector2 next = (Vector2)transform.position + delta;
            next.x = Mathf.Clamp(next.x, minX, maxX);
            next.y = Mathf.Clamp(next.y, minY, maxY);

            transform.position = next;
        }

        private void HandleShooting()
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f) return;
            if (bulletPrefab == null || firePoint == null) return;

            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity).SetActive(true);
            fireTimer = fireRate;
        }

        private void HandleInvincibility()
        {
            if (invincibilityTimer <= 0f) return;

            invincibilityTimer -= Time.deltaTime;

            if (sr != null)
            {
                float a = (Mathf.Sin(Time.time * blinkRate) + 1f) * 0.5f;
                Color c = sr.color;
                c.a = Mathf.Lerp(0.3f, 1f, a);
                sr.color = c;
            }

            if (invincibilityTimer <= 0f && sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }

        public bool IsInvincible => invincibilityTimer > 0f;

        public void Init(GameObject bulletPrefab, Transform firePoint)
        {
            this.bulletPrefab = bulletPrefab;
            this.firePoint = firePoint;
        }

        public void ApplyDamage(int dmg)
        {
            if (IsInvincible) return;
            health.TakeDamage(dmg);
            invincibilityTimer = invincibilitySeconds;
        }

        public void Heal(int amount) => health.Heal(amount);

        private void OnHealthChanged(int cur, int max)
        {
            GameEvents.RaisePlayerHealthChanged(cur, max);
        }

        private void OnPlayerDied()
        {
            GameEvents.RaisePlayerDied();
        }
    }
}
