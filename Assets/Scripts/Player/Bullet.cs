using UnityEngine;
using ZorianyRaid.Enemies;

namespace ZorianyRaid.Player
{
    /// <summary>
    /// Куля гравця. Рухається вгору з постійною швидкістю,
    /// знищує себе за межами екрана або при влученні в ціль із Health.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 12f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifetime = 3f;

        private float age;

        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void Update()
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            age += Time.deltaTime;
            if (age >= lifetime) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Куля влучає у ворогів і астероїди
            var hittable = other.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.OnHitByBullet(damage);
                Destroy(gameObject);
            }
        }
    }
}
