# Зоряний Рейд — Unity Project

2D вертикальний космічний шутер для дисципліни «Фреймворки для розроблення комп'ютерних ігор».

## Керування
| Дія          | Клавіша              |
|--------------|----------------------|
| Рух корабля  | **WASD** / стрілки   |
| Стрільба     | автоматична          |
| Пауза        | **Esc** / **P**      |
| Рестарт      | **R** (після Game Over) |

## Структура проєкту
```
Assets/
└── Scripts/
    ├── Core/         GameEvents, GameManager, Health, SceneBootstrap, SpriteFactory
    ├── Player/       PlayerController, Bullet
    ├── Enemies/      Enemy (Drone), Asteroid, IHittable, DownwardMover
    ├── Spawning/     WaveManager, SimpleSpawner
    ├── Pickups/      PowerUp
    ├── UI/           UIManager, ScoreManager
    └── Background/   BackgroundScroller, StarTwinkle
```
