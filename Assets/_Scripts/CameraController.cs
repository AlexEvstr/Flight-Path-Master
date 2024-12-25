using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform plane;
    [SerializeField] private GameObject enterPanel;
    private float cameraMoveDuration = 2f; // Длительность движения камеры в секундах
    private Vector3 startPosition = new Vector3(0, 1, 0); // Начальная позиция камеры
    private Vector3 targetPosition = new Vector3(0, 10, 0); // Целевая позиция камеры
    private PlaneController planeController; // Ссылка на скрипт управления самолетом

    void Start()
    {
        transform.position = startPosition;
        planeController = plane.GetComponent<PlaneController>();

        // Убедимся, что самолет не двигается до начала
        if (planeController != null)
        {
            planeController.enabled = false;
        }
    }

    public void StartGame()
    {
        enterPanel.SetActive(false);
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < cameraMoveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / cameraMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующий кадр
        }

        // Гарантируем точное положение камеры
        transform.position = targetPosition;

        // Включаем управление самолетом
        if (planeController != null)
        {
            planeController.enabled = true;
        }
    }
}
