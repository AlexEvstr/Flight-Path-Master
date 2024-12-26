using UnityEngine;

public class SpriteShapeValidator : MonoBehaviour
{
    private SpriteRenderer shapeSpriteRenderer;
    private Texture2D shapeTexture;

    public SpriteRenderer ShapeSpriteRenderer => shapeSpriteRenderer;
    public Texture2D ShapeTexture => shapeTexture;


    private int totalShapePoints;
    private bool[,] coverageMap;

    private int coveredPoints = 0;

    [SerializeField] private float inkNeeded;

    void Awake()
    {
        shapeSpriteRenderer = GetComponent<SpriteRenderer>();

        if (shapeSpriteRenderer.sprite != null)
        {
            shapeTexture = Instantiate(shapeSpriteRenderer.sprite.texture);

            coverageMap = new bool[shapeTexture.width, shapeTexture.height];

            totalShapePoints = CountShapePixels();
        }
    }

    private int CountShapePixels()
    {
        int count = 0;
        int step = 1;

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
    private bool IsPixelOpaque(int x, int y)
    {
        Color pixelColor = shapeTexture.GetPixel(x, y);
        return pixelColor.a > 0.5f;
    }

    public void RegisterPoint(Vector3 worldPosition, float lineThickness)
    {
        if (shapeTexture == null) return;

        Vector2 localPosition = shapeSpriteRenderer.transform.InverseTransformPoint(worldPosition);

        Vector2 spritePosition = new Vector2(
            localPosition.x + shapeSpriteRenderer.sprite.bounds.extents.x,
            localPosition.y + shapeSpriteRenderer.sprite.bounds.extents.y
        );

        int textureX = Mathf.FloorToInt(spritePosition.x * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);
        int textureY = Mathf.FloorToInt(spritePosition.y * shapeTexture.height / shapeSpriteRenderer.sprite.bounds.size.y);

        if (textureX < 0 || textureY < 0 || textureX >= shapeTexture.width || textureY >= shapeTexture.height)
            return;

        int thicknessRadius = Mathf.CeilToInt(lineThickness * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);

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

        Vector2 localPosition = shapeSpriteRenderer.transform.InverseTransformPoint(worldPosition);

        Vector2 spritePosition = new Vector2(
            localPosition.x + shapeSpriteRenderer.sprite.bounds.extents.x,
            localPosition.y + shapeSpriteRenderer.sprite.bounds.extents.y
        );

        int textureX = Mathf.FloorToInt(spritePosition.x * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);
        int textureY = Mathf.FloorToInt(spritePosition.y * shapeTexture.height / shapeSpriteRenderer.sprite.bounds.size.y);

        if (textureX < 0 || textureY < 0 || textureX >= shapeTexture.width || textureY >= shapeTexture.height)
            return false;

        int thicknessRadius = Mathf.CeilToInt(lineThickness * shapeTexture.width / shapeSpriteRenderer.sprite.bounds.size.x);

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
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public float CalculateInkNeeded()
    {
        return inkNeeded;
    }
}