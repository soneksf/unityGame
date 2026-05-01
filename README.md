# Зоряний Рейд — Unity Project

2D вертикальний космічний шутер для дисципліни «Фреймворки для розроблення комп'ютерних ігор».

## Швидкий старт

### Крок 1. Відкрити проєкт у Unity
1. Встановити **Unity 2022.3 LTS** (через Unity Hub).
2. У Unity Hub: **Add → Add project from disk** → вибрати папку `ZorianyRaid`.
3. Натиснути на проєкт у списку — Unity відкриє редактор. Перший імпорт займе 1–3 хвилини
   (Unity згенерує всі службові файли: Library, Temp, ScriptAssemblies тощо).

### Крок 2. Створити сцену (один раз)
Сцена не входить у git, бо `Bootstrap`-скрипт будує всю ієрархію програмно.
1. **File → New Scene → Basic 2D** (або 2D Core).
2. Видалити все із сцени (включно з Main Camera — `SceneBootstrap` створить власну).
3. **GameObject → Create Empty**, назвати `Bootstrap`.
4. На об'єкті `Bootstrap` натиснути **Add Component → Scene Bootstrap**.
5. **File → Save As** → `Assets/Scenes/Game.unity`.
6. Натиснути ▶️ **Play**.

### Альтернатива (для другого етапу)
В Inspector на `SceneBootstrap`:
- `Use Wave Manager = true` — основний ігровий цикл з хвилями (Етап 2).
- `Use Wave Manager = false` — простий спавнер для базового тестування (Етап 1).

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

## Архітектура
- **Шина подій** (`GameEvents`) — слабкий зв'язок: гравець, вороги, бонуси кидають події;
  UI і ScoreManager на них реагують. Жоден скрипт не знає про конкретного підписника.
- **Сінглтони** — `GameManager` (стан гри), `ScoreManager` (бали і рекорди).
- **`IHittable`** — куля знає тільки про інтерфейс, тому той самий код бомбить
  і дронів, і астероїди.
- **`SceneBootstrap`** — будує всю сцену в Awake, тому проєкт працює без імпорту арту.
  Вся графіка — кольорові примітиви, що генеруються через `SpriteFactory`.

## Чому скрипт-bootstrap, а не фабричні prefab-и?
Це навчальний прототип — мета показати ігрову логіку. Bootstrap дозволяє:
- запустити гру одним кліком, без налаштування Inspector;
- тримати весь проєкт у git без LFS і двійкових .prefab-файлів;
- швидко переключатися між «етапом 1» (просто спавнер) і «етапом 2» (хвилі)
  через одну галочку в Inspector.

У production-проєкті, звісно, краще мати справжні префаби з художніми ассетами
і ScriptableObject-конфігами, але для лабораторної цей підхід доречніший.
