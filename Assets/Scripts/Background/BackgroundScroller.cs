using UnityEngine;

namespace ZorianyRaid.Background
{
    /// <summary>
    /// Простий parallax-скролер фону. Зсуває material offset за віссю Y
    /// або переміщує два спрайти на повтор. У цьому компоненті — варіант
    /// з material offset, найдешевший за продуктивністю.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class BackgroundScroller : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 0.05f;
        [SerializeField] private string textureProperty = "_MainTex";

        private Material mat;
        private Vector2 offset;

        private void Awake()
        {
            // Інстанціюємо матеріал, щоб не псувати ассет
            mat = GetComponent<Renderer>().material;
        }

        private void Update()
        {
            offset.y -= scrollSpeed * Time.deltaTime;
            mat.SetTextureOffset(textureProperty, offset);
        }
    }
}
