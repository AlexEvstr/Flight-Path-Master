using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private float forwardSpeed = 1.5f; // Скорость полета вперед
    private float rotationSpeed = 25f; // Скорость вращения самолета

    void Update()
    {
        MoveForward();
        HandleRotation();
    }

    void MoveForward()
    {
        // Постоянное движение вперед
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                // Получаем позицию касания
                Vector2 touchDelta = touch.deltaPosition;

                // Преобразуем движение пальца в поворот по оси Y
                float rotationY = touchDelta.x * rotationSpeed * Time.deltaTime;

                // Поворачиваем самолет
                transform.Rotate(0, rotationY, 0);
            }
        }
    }
}
