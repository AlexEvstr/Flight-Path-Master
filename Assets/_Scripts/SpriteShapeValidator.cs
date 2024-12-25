using UnityEngine;

public class SpriteShapeValidator : MonoBehaviour
{
    private SpriteRenderer shapeSpriteRenderer;
    private Texture2D shapeTexture;

    private int totalShapePoints; // Всего точек на форме
    private int coveredPoints; // Покрытые игроком точки

    private bool[,] coverageMap; // Карта покрытия формы

    void Start()
    {
        shapeSpriteRenderer = GetComponent<SpriteRenderer>();

        if (shapeSpriteRenderer.sprite != null)
        {
            // Создаем копию текстуры для изменений
            shapeTexture = Instantiate(shapeSpriteRenderer.sprite.texture);

            // Считаем общее количество точек на форме
            totalShapePoints = CountShapePixels();
            coveredPoints = 0;

            // Инициализируем карту покрытия
            coverageMap = new bool[shapeTexture.width, shapeTexture.height];

            Debug.Log($"[DEBUG] Всего точек на форме: {totalShapePoints}");
        }
        else
        {
            Debug.LogError("У объекта нет спрайта!");
        }
    }

    private int CountShapePixels()
    {
        int count = 0;

        for (int x = 0; x < shapeTexture.width; x++)
        {
            for (int y = 0; y < shapeTexture.height; y++)
            {
                if (IsPixelOpaque(x, y))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsPixelOpaque(int x, int y)
    {
        Color pixelColor = shapeTexture.GetPixel(x, y);
        return pixelColor.a > 0.5f; // Учитываем только непрозрачные пиксели
    }

    public void RegisterPoint(Vector3 worldPosition, float lineThickness)
    {
        if (shapeTexture == null) return;

        // Переводим мировые координаты в текстурные
        Vector2 localPosition = shapeSpriteRenderer.transform.InverseTransformPoint(worldPosition);

        Vector2 spritePosition = new Vector2(
            localPosition.x + shapeSpriteRenderer.sprite.bounds.extents.x,
            localPosition.y + shapeSpriteRenderer.sprite.bounds.extents.y
        );

        int textureX = Mathf.FloorToInt(spritePosition.x * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);
        int textureY = Mathf.FloorToInt(spritePosition.y * shapeTexture.height / shapeSpriteRenderer.sprite.bounds.size.y);

        if (textureX < 0 || textureY < 0 || textureX >= shapeTexture.width || textureY >= shapeTexture.height)
            return;

        // Рассчитываем радиус проверки (толщина линии в пикселях)
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


    public float GetProgress()
    {
        if (totalShapePoints == 0) return 0;
        return (float)coveredPoints / totalShapePoints * 100f;
    }

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

        // Учитываем толщину линии
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

}
