using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrailDrawer : MonoBehaviour
{
    public float minDistance = 0.1f; // Минимальное расстояние для новой точки
    public float lineThickness = 0.5f; // Толщина линии

    private LineRenderer lineRenderer;
    private Vector3 lastPosition;
    private bool isDrawing = false; // Начато ли рисование

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
    }

    void Update()
    {
        if (!isDrawing)
        {
            // Проверяем первый контакт
            if (validator.IsPointOnShape(transform.position, lineThickness))
            {
                isDrawing = true; // Включаем рисование
                Debug.Log("[DEBUG] Первый контакт с формой!");
                AddPoint(transform.position); // Добавляем первую точку
            }
        }
        else
        {
            // Если рисование начато, продолжаем добавлять точки
            if (Vector3.Distance(transform.position, lastPosition) > minDistance)
            {
                AddPoint(transform.position);
                validator.RegisterPoint(transform.position, lineThickness);
            }
        }
    }

    void AddPoint(Vector3 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
        lastPosition = position;
    }
}
