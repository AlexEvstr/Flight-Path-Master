using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class TrailDrawer : MonoBehaviour
{
    public float minDistance = 0.1f;
    public float inkUsagePerUnit = 1f;
    public Text inkText;
    public Text progressText;
    public Image inkBar;

    private LineRenderer lineRenderer;
    private Vector3 lastPosition;
    private bool isDrawing = false;
    private float currentInk;
    private float maxInk;

    private SpriteShapeValidator validator;
    private bool _isGameFinished;
    [SerializeField] private CustomWindowManager _customWindowManager;
    [SerializeField] private GameObject[] _levels;

    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        _levels[currentLevel - 1].SetActive(true);
        _isGameFinished = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lastPosition = transform.position;

        validator = FindObjectOfType<SpriteShapeValidator>();
        if (validator == null)
        {

        }

        float pixelWidthInWorld = validator.ShapeSpriteRenderer.bounds.size.x / validator.ShapeTexture.width;
        inkUsagePerUnit = lineRenderer.startWidth * pixelWidthInWorld;

        if (validator != null)
        {
            maxInk = validator.CalculateInkNeeded();
            maxInk *= 1.1f;
            currentInk = maxInk;
        }

        UpdateUI();
    }

    void Update()
    {
        if (!isDrawing)
        {
            if (validator.IsPointOnShape(transform.position, lineRenderer.startWidth))
            {
                isDrawing = true;
                AddPoint(transform.position);
            }
        }
        else
        {
            if (currentInk <= 0f)
            {
                return;
            }

            if (Vector3.Distance(transform.position, lastPosition) > minDistance)
            {
                float distance = Vector3.Distance(lastPosition, transform.position);
                float inkRequired = distance * inkUsagePerUnit;

                if (currentInk >= inkRequired)
                {
                    currentInk -= inkRequired;
                    AddPoint(transform.position);
                    validator.RegisterPoint(transform.position, lineRenderer.startWidth);
                    UpdateUI();
                }
                else
                {
                    if (!_isGameFinished)
                    {
                        _isGameFinished = true;
                        float shapeProgress = validator.GetProgress();
                        if (shapeProgress >= 80)
                        {
                            _customWindowManager.ShowVictoryPopup();
                        }
                        else
                        {
                            _customWindowManager.ShowDefeatPopup();
                        }
                    }
                    
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
        // Рассчитываем пропорцию для чернил
        float inkFill = Mathf.Clamp01(currentInk / maxInk); // От 0 до 1
        float shapeProgress = validator.GetProgress(); // Прогресс закрашивания в процентах

        // Обновляем текстовые поля
        if (inkText != null)
        {
            inkText.text = $"{Mathf.RoundToInt(inkFill * 100)}%"; // Процент оставшихся чернил
        }

        if (progressText != null)
        {
            progressText.text = $"{shapeProgress:F1}%"; // Прогресс формы в процентах
        }

        // Если есть бар чернил (Image), обновляем его fillAmount
        if (inkBar != null)
        {
            inkBar.fillAmount = inkFill; // Заполняем бар чернил
        }
    }
}