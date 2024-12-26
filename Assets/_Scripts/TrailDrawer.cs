using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class TrailDrawer : MonoBehaviour
{
    public float minDistance = 0.1f; // Минимальное расстояние для новой точки
    public float inkUsagePerUnit = 1f; // Коэффициент расхода чернил на единицу длины
    public Text inkText; // UI-элемент для отображения количества чернил
    public Text progressText; // UI-элемент для отображения прогресса закрашивания

    private LineRenderer lineRenderer;
    private Vector3 lastPosition;
    private bool isDrawing = false; // Начато ли рисование
    private float currentInk; // Текущее количество чернил
    private float maxInk; // Максимально доступное количество чернил

    private SpriteShapeValidator validator;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lastPosition = transform.position;

        validator = FindObjectOfType<SpriteShapeValidator>();
        if (validator == null)
        {
            Debug.LogError("SpriteShapeValidator не найден!");
        }

        // Рассчитываем расход чернил на единицу длины
        float pixelWidthInWorld = validator.ShapeSpriteRenderer.bounds.size.x / validator.ShapeTexture.width;
        inkUsagePerUnit = lineRenderer.startWidth * pixelWidthInWorld; // Ширина линии в мировых координатах
        Debug.Log($"[DEBUG] Расход чернил на единицу длины: {inkUsagePerUnit:F3}");


        // Расчет общего количества чернил
        if (validator != null)
        {
            maxInk = validator.CalculateInkNeeded();
            maxInk *= 1.1f; // 10% запас
            currentInk = maxInk;
            Debug.Log($"[DEBUG] Всего чернил (с запасом): {maxInk:F2}");
        }

        UpdateUI();
    }








    void Update()
    {
        if (!isDrawing)
        {
            // Проверяем первый контакт с формой
            if (validator.IsPointOnShape(transform.position, lineRenderer.startWidth))
            {
                isDrawing = true;
                AddPoint(transform.position); // Добавляем первую точку
                Debug.Log("[DEBUG] Первый контакт с формой!");
            }
        }
        else
        {
            // Если чернила закончились, прекращаем рисовать
            if (currentInk <= 0f)
            {
                Debug.Log("[DEBUG] Чернила закончились!");
                return;
            }

            // Если рисование начато, продолжаем добавлять точки
            if (Vector3.Distance(transform.position, lastPosition) > minDistance)
            {
                float distance = Vector3.Distance(lastPosition, transform.position);
                float inkRequired = distance * inkUsagePerUnit;

                // Проверяем, хватает ли чернил
                if (currentInk >= inkRequired)
                {
                    currentInk -= inkRequired;
                    AddPoint(transform.position);
                    validator.RegisterPoint(transform.position, lineRenderer.startWidth);
                    UpdateUI();
                }
                else
                {
                    Debug.Log("[DEBUG] Недостаточно чернил для следующей точки!");
                }
            }
        }
    }

    void AddPoint(Vector3 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        lastPosition = position;
    }

    void UpdateUI()
    {
        // Рассчитываем проценты оставшихся чернил и прогресса закрашивания
        float inkPercentage = Mathf.Max(currentInk / maxInk * 100f, 0);
        float shapeProgress = validator.GetProgress();

        if (inkText != null)
        {
            inkText.text = $"Чернила: {inkPercentage:F1}%";
        }

        if (progressText != null)
        {
            progressText.text = $"Закрашено: {shapeProgress:F1}%";
        }

        Debug.Log($"[DEBUG] Остаток чернил: {inkPercentage:F1}%, Закрашено: {shapeProgress:F1}%");
    }


}
