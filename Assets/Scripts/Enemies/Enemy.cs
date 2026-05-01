using UnityEngine;
using ZorianyRaid.Core;
using ZorianyRaid.Player;

namespace ZorianyRaid.Enemies
{
    /// <summary>
    /// Ворог-Дрон. 1 HP за замовчуванням, рухається вниз.
    /// При зіткненні з гравцем — наносить урон і знищується.
    /// При знищенні від кулі — нараховує бали та може випустити бонус здоров'я.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour, IHittable
    {
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private int scoreReward = 100;

        [Header("Дроп бонусу")]
        [SerializeField] private GameObject powerUpPrefab;
        [Range(0f, 1f)]
        [SerializeField] private float powerUpDropChance = 0.1f;

        private Health health;

        public void SetPowerUpPrefab(GameObject prefab) { this.powerUpPrefab = prefab; }

        private void Awake()
        {
            health = GetComponent<Health>();
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnEnable()
        {
            health.Died += HandleDestroyedByBullet;
        }

        private void OnDisable()
        {
            health.Died -= HandleDestroyedByBullet;
        }

        public void OnHitByBullet(int damage) => health.TakeDamage(damage);

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsInvincible)
            {
                player.ApplyDamage(contactDamage);
                // Зіткнення з гравцем не дає очок: вмикаємо знищення без нагороди
                Destroy(gameObject);
            }
        }

        private void HandleDestroyedByBullet()
        {
            GameEvents.RaiseEnemyDestroyed(scoreReward);
            TryDropPowerUp();
        }

        private void TryDropPowerUp()
        {
            if (powerUpPrefab == null) return;
            if (Random.value <= powerUpDropChance)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity).SetActive(true);
            }
        }
    }
}
