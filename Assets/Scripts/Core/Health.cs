using System;
using UnityEngine;

namespace ZorianyRaid.Core
{
    /// <summary>
    /// Універсальний компонент здоров'я. Використовується гравцем,
    /// ворогами та астероїдами. Має локальні події для конкретного власника.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 1;
        [SerializeField] private bool destroyOnDeath = true;

        private int currentHealth;
        private bool isDead;

        public int Current => currentHealth;
        public int Max => maxHealth;
        public bool IsDead => isDead;

        public event Action<int, int> Changed;
        public event Action Died;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void SetMax(int value, bool refill = true)
        {
            maxHealth = Mathf.Max(1, value);
            if (refill) currentHealth = maxHealth;
            Changed?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (isDead || amount <= 0) return;

            currentHealth = Mathf.Max(0, currentHealth - amount);
            Changed?.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
            {
                isDead = true;
                Died?.Invoke();
                if (destroyOnDeath) Destroy(gameObject);
            }
        }

        public void Heal(int amount)
        {
            if (isDead || amount <= 0) return;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            Changed?.Invoke(currentHealth, maxHealth);
        }
    }
}
