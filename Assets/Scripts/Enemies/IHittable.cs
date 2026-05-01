namespace ZorianyRaid.Enemies
{
    /// <summary>
    /// Контракт для будь-чого, що може отримати влучання від кулі гравця.
    /// Реалізують Enemy і Asteroid.
    /// </summary>
    public interface IHittable
    {
        void OnHitByBullet(int damage);
    }
}
