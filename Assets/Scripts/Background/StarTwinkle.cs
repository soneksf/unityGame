using UnityEngine;

namespace ZorianyRaid.Background
{
    /// <summary>
    /// Допоміжний компонент мерехтіння зірок на тлі.
    /// Кожна зірка має власну фазу — гарантує асинхронний візуал.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class StarTwinkle : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;

        private SpriteRenderer sr;
        private float phase;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            phase = Random.Range(0f, 6.28f);
        }

        private void Update()
        {
            float a = 0.5f + 0.5f * Mathf.Sin(Time.time * speed + phase);
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }
}
