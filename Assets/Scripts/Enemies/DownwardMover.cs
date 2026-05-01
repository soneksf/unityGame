using UnityEngine;

namespace ZorianyRaid.Enemies
{
    /// <summary>
    /// Спрощений рух «зверху вниз» для ворогів та астероїдів.
    /// Знищує об'єкт, коли він повністю покидає нижню межу екрана.
    /// </summary>
    public class DownwardMover : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float bottomDespawnY = -6f;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        private void Update()
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y < bottomDespawnY) Destroy(gameObject);
        }
    }
}
