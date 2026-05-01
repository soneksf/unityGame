using UnityEngine;
using ZorianyRaid.Core;
using ZorianyRaid.Player;

namespace ZorianyRaid.Enemies
{
    /// <summary>
    /// Астероїд: рухається вниз, знищується з одного попадання,
    /// при зіткненні з гравцем наносить урон і зникає.
    /// Не випускає бонусів.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Collider2D))]
    public class Asteroid : MonoBehaviour, IHittable
    {
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private int scoreReward = 50;

        private Health health;

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
                Destroy(gameObject);
            }
        }

        private void HandleDestroyedByBullet()
        {
            GameEvents.RaiseAsteroidDestroyed(scoreReward);
        }
    }
}
