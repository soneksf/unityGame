using System.Collections;
using UnityEngine;
using ZorianyRaid.Enemies;

namespace ZorianyRaid.Spawning
{
    /// <summary>
    /// Простий спавнер для тестів першого етапу.
    /// Кидає по одному об'єкту через фіксований інтервал у випадкове X.
    /// Не керує складністю — це робить WaveManager у другому етапі.
    /// </summary>
    public class SimpleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float interval = 1.5f;
        [SerializeField] private float spawnY = 6f;
        [SerializeField] private float xMin = -7f;
        [SerializeField] private float xMax = 7f;
        [SerializeField] private float speed = 2f;

        public void Init(GameObject prefab) { this.prefab = prefab; }

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            var wait = new WaitForSeconds(interval);
            while (true)
            {
                if (prefab != null)
                {
                    Vector3 pos = new Vector3(Random.Range(xMin, xMax), spawnY, 0f);
                    GameObject go = Instantiate(prefab, pos, Quaternion.identity);
                    go.SetActive(true);
                    var mover = go.GetComponent<DownwardMover>();
                    if (mover != null) mover.Speed = speed;
                }
                yield return wait;
            }
        }
    }
}
