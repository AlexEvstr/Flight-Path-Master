using UnityEngine;

public class ScreenBoundaryChecker : MonoBehaviour
{
    public Camera mainCamera;
    public Transform target;

    private CustomWindowManager _customWindowManager;

    private bool gameOverTriggered = false;

    void Start()
    {
        _customWindowManager = GetComponent<CustomWindowManager>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (gameOverTriggered || target == null || mainCamera == null) return;

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(target.position);

        if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1)
        {
            _customWindowManager.ShowDefeatPopup();
            gameOverTriggered = true;
        }
    }
}