using UnityEngine;
using ZorianyRaid.Core;
using ZorianyRaid.Player;

namespace ZorianyRaid.Pickups
{
    /// <summary>
    /// Зелений бонус здоров'я. Падає вниз, при контакті з гравцем
    /// додає 1 HP (обмежено максимумом) і нараховує бонусні бали.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PowerUp : MonoBehaviour
    {
        [SerializeField] private int healAmount = 1;
        [SerializeField] private int scoreReward = 25;

        [Header("Анімація пульсації")]
        [SerializeField] private float pulseAmplitude = 0.1f;
        [SerializeField] private float pulseFrequency = 3f;

        private Vector3 baseScale;

        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
            baseScale = transform.localScale;
        }

        private void Update()
        {
            float k = 1f + Mathf.Sin(Time.time * pulseFrequency) * pulseAmplitude;
            transform.localScale = baseScale * k;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null) return;

            player.Heal(healAmount);
            GameEvents.RaisePowerUpCollected();
            GameEvents.RaiseScoreAwarded(scoreReward);
            Destroy(gameObject);
        }
    }
}
