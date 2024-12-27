using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private float forwardSpeed = 1.5f;
    private float rotationSpeed = 25f;

    void Update()
    {
        MoveForward();
        HandleRotation();
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                Vector2 touchDelta = touch.deltaPosition;

                float rotationY = touchDelta.x * rotationSpeed * Time.deltaTime;

                transform.Rotate(0, rotationY, 0);
            }
        }
    }
}