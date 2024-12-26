using UnityEngine;

public class SpriteShapeValidator : MonoBehaviour
{
    private SpriteRenderer shapeSpriteRenderer;
    private Texture2D shapeTexture;

    public SpriteRenderer ShapeSpriteRenderer => shapeSpriteRenderer;
    public Texture2D ShapeTexture => shapeTexture;


    private int totalShapePoints; // Всего точек на форме
    private bool[,] coverageMap; // Карта покрытия формы

    private int coveredPoints = 0; // Количество покрытых точек

    [SerializeField] private float inkNeeded;

    void Awake()
    {
        shapeSpriteRenderer = GetComponent<SpriteRenderer>();

        if (shapeSpriteRenderer.sprite != null)
        {
            // Создаем копию текстуры для работы
            shapeTexture = Instantiate(shapeSpriteRenderer.sprite.texture);

            // Инициализируем карту покрытия
            coverageMap = new bool[shapeTexture.width, shapeTexture.height];

            // Подсчитываем общее количество непрозрачных пикселей
            totalShapePoints = CountShapePixels();
            Debug.Log($"[DEBUG] Всего точек на форме: {totalShapePoints}");
        }
        else
        {
            Debug.LogError("У объекта нет спрайта!");
        }
    }

    /// <summary>
    /// Считает количество непрозрачных пикселей на текстуре формы.
    /// </summary>
    private int CountShapePixels()
    {
        int count = 0;
        int step = 1; // Шаг итерации (для оптимизации можно увеличить)

        for (int x = 0; x < shapeTexture.width; x += step)
        {
            for (int y = 0; y < shapeTexture.height; y += step)
            {
                if (IsPixelOpaque(x, y))
                {
                    count++;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Проверяет, является ли пиксель непрозрачным.
    /// </summary>
    private bool IsPixelOpaque(int x, int y)
    {
        Color pixelColor = shapeTexture.GetPixel(x, y);
        return pixelColor.a > 0.5f; // Считаем пиксель непрозрачным, если его прозрачность больше 0.5
    }

    /// <summary>
    /// Регистрирует попадание точки линии на форму.
    /// </summary>
    public void RegisterPoint(Vector3 worldPosition, float lineThickness)
    {
        if (shapeTexture == null) return;

        // Переводим мировые координаты в координаты текстуры
        Vector2 localPosition = shapeSpriteRenderer.transform.InverseTransformPoint(worldPosition);

        Vector2 spritePosition = new Vector2(
            localPosition.x + shapeSpriteRenderer.sprite.bounds.extents.x,
            localPosition.y + shapeSpriteRenderer.sprite.bounds.extents.y
        );

        int textureX = Mathf.FloorToInt(spritePosition.x * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);
        int textureY = Mathf.FloorToInt(spritePosition.y * shapeTexture.height / shapeSpriteRenderer.sprite.bounds.size.y);

        if (textureX < 0 || textureY < 0 || textureX >= shapeTexture.width || textureY >= shapeTexture.height)
            return;

        // Учитываем радиус проверки (толщина линии в пикселях)
        int thicknessRadius = Mathf.CeilToInt(lineThickness * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);

        // Проверяем все точки в радиусе толщины линии
        for (int offsetX = -thicknessRadius; offsetX <= thicknessRadius; offsetX++)
        {
            for (int offsetY = -thicknessRadius; offsetY <= thicknessRadius; offsetY++)
            {
                int checkX = textureX + offsetX;
                int checkY = textureY + offsetY;

                if (checkX >= 0 && checkY >= 0 && checkX < shapeTexture.width && checkY < shapeTexture.height)
                {
                    if (IsPixelOpaque(checkX, checkY) && !coverageMap[checkX, checkY])
                    {
                        coverageMap[checkX, checkY] = true;
                        coveredPoints++;
                        Debug.Log($"[DEBUG] Покрытая точка: ({checkX}, {checkY}). Покрыто: {coveredPoints}/{totalShapePoints} ({GetProgress():F1}%)");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Рассчитывает процент покрытия формы.
    /// </summary>
    public float GetProgress()
    {
        if (totalShapePoints == 0) return 0;
        return (float)coveredPoints / totalShapePoints * 100f;
    }

    /// <summary>
    /// Проверяет, находится ли самолет на форме.
    /// </summary>
    public bool IsPointOnShape(Vector3 worldPosition, float lineThickness)
    {
        if (shapeTexture == null) return false;

        // Переводим мировые координаты в текстурные
        Vector2 localPosition = shapeSpriteRenderer.transform.InverseTransformPoint(worldPosition);

        Vector2 spritePosition = new Vector2(
            localPosition.x + shapeSpriteRenderer.sprite.bounds.extents.x,
            localPosition.y + shapeSpriteRenderer.sprite.bounds.extents.y
        );

        int textureX = Mathf.FloorToInt(spritePosition.x * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);
        int textureY = Mathf.FloorToInt(spritePosition.y * shapeTexture.height / shapeSpriteRenderer.sprite.bounds.size.y);

        if (textureX < 0 || textureY < 0 || textureX >= shapeTexture.width || textureY >= shapeTexture.height)
            return false;

        // Учитываем радиус проверки (толщина линии в пикселях)
        int thicknessRadius = Mathf.CeilToInt(lineThickness * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);

        // Проверяем все точки в радиусе толщины линии
        for (int offsetX = -thicknessRadius; offsetX <= thicknessRadius; offsetX++)
        {
            for (int offsetY = -thicknessRadius; offsetY <= thicknessRadius; offsetY++)
            {
                int checkX = textureX + offsetX;
                int checkY = textureY + offsetY;

                if (checkX >= 0 && checkY >= 0 && checkX < shapeTexture.width && checkY < shapeTexture.height)
                {
                    if (IsPixelOpaque(checkX, checkY))
                    {
                        return true; // Найдена точка на форме
                    }
                }
            }
        }

        return false; // Ни одна точка не попала на форму
    }

    /// <summary>
    /// Рассчитывает необходимое количество чернил для покрытия формы.
    /// </summary>
    public float CalculateInkNeeded()
    {


        return inkNeeded;
    }












}
